namespace webapiV2.Services.Accounts;

using AutoMapper;
using BCrypt.Net;
using webapiV2.Entities;
using webapiV2.Authorization;
using webapiV2.Models.Accounts;
using webapiV2.Helpers;
using webapiV2.Services.Emails;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
//using System.Security.Claims;

public class AccountService : IAccountService {
    private readonly DataContext _context;
    private IJwtUtils _jwtUtils;
    private readonly IMapper _mapper;
    private IEmailService _emailService;
    private readonly AppSettings _appSettings;

    public AccountService(DataContext context, IJwtUtils jwtUtils, IMapper mapper, IEmailService emailService, IOptions<AppSettings> appSettings) {
        _context = context;
        _jwtUtils = jwtUtils;
        _mapper = mapper;
        _emailService = emailService;
        _appSettings = appSettings.Value;
    }

    public AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress) {
        var account = _context.Accounts.SingleOrDefault(x => x.Email == model.Email);
        // validate
        //! Isverfied yani e-mail adresi doğrulanmamış kullanıcı olduğunda farklı mesaj ve kodu döndürülmeli.
        if (account == null || !account.IsVerified || !BCrypt.Verify(model.Password, account.PasswordHash) || account.IsDisabled)
            throw new AppException("Email adresi veya şifre yanlış. Lütfen sistem yöneticiniz ile görüşünüz!");

        // authentication successful so generate jwt token and refresh tokens
        var jwtToken = _jwtUtils.GenerateJwtToken(account);
        var refreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);
        account.RefreshTokens.Add(refreshToken);

        // remove old refresh tokens from user
        removeOldRefreshTokens(account);

        // save changes to db
        _context.Update(account);
        _context.SaveChanges();

        var response = _mapper.Map<AuthenticateResponse>(account);
        response.JwtToken = jwtToken;
        response.RefreshToken = refreshToken.Token;
        return response;
    }

    public AuthenticateResponse RefreshToken(string token, string ipAddress) {
        var account = getAccountByRefreshToken(token);
        var refreshToken = account.RefreshTokens.Single(x => x.Token == token);

        if (refreshToken.IsRevoked) {
            // revoke all descendant tokens in case this token has been compromised
            revokeDescendantRefreshTokens(refreshToken, account, ipAddress, $"İptal edilen yada yeniden denene Token: {token}");
            _context.Update(account);
            _context.SaveChanges();
        }

        if (!refreshToken.IsActive)
            throw new AppException("Geçersiz token");

        // replace old refresh token with a new one (rotate token)
        var newRefreshToken = rotateRefreshToken(refreshToken, ipAddress);
        account.RefreshTokens.Add(newRefreshToken);

        // remove old refresh tokens from account
        removeOldRefreshTokens(account);

        // save changes to db
        _context.Update(account);
        _context.SaveChanges();

        // generate new jwt
        var jwtToken = _jwtUtils.GenerateJwtToken(account);

        // return data in authenticate response object
        var response = _mapper.Map<AuthenticateResponse>(account);
        response.JwtToken = jwtToken;
        response.RefreshToken = newRefreshToken.Token;
        return response;
    }

    public void RevokeToken(string token, string ipAddress) {
        var account = getAccountByRefreshToken(token);
        var refreshToken = account.RefreshTokens.Single(x => x.Token == token);

        if (!refreshToken.IsActive)
            throw new AppException("Geçersiz token");

        // revoke token and save
        revokeRefreshToken(refreshToken, ipAddress, "Revoked without replacement");
        _context.Update(account);
        _context.SaveChanges();
    }

    public void Register(RegisterRequest model, string origin) {
        // validate
        if (_context.Accounts.Any(x => x.Email == model.Email)) {
            //throw new AppException("Email adresi '" + model.Email + "' sistemde mevcut!");
            // send already registered error in email to prevent account enumeration
            sendAlreadyRegisteredEmail(model.Email, origin);
            return;
        }

        // map model to new account object
        var account = _mapper.Map<Account>(model);

        // first registered account is an admin
        // var isFirstAccount = _context.Accounts.Count() == 0;
        // account.Role = isFirstAccount ? Role.Admin : Role.User;
        account.Role = Role.User;
        account.Created = DateTime.UtcNow;
        account.IsDisabled = false;
        account.VerificationToken = _jwtUtils.getUniqueTokenGuid(); //hm.generateVerificationToken();

        // hash password
        account.PasswordHash = BCrypt.HashPassword(model.Password);

        // save account
        _context.Accounts.Add(account);
        _context.SaveChanges();

        // email gönder
        sendVerificationEmail(account, origin);
    }

    public void VerifyEmail(string token) {
        var account = _context.Accounts.SingleOrDefault(x => x.VerificationToken == token);

        if (account == null)
            throw new AppException("Doğrulama başarısız oldu");

        account.Verified = DateTime.UtcNow;
        account.VerificationToken = null;

        _context.Accounts.Update(account);
        _context.SaveChanges();
    }

    public void ForgotPassword(ForgotPasswordRequest model, string origin) {
        var account = _context.Accounts.SingleOrDefault(x => x.Email == model.Email);

        // always return ok response to prevent email enumeration
        if (account == null) return;

        // create reset token that expires after 1 day
        account.ResetToken = _jwtUtils.getUniqueTokenGuid(); //hm.generateResetToken();
        account.ResetTokenExpires = DateTime.UtcNow.AddDays(1);

        _context.Accounts.Update(account);
        _context.SaveChanges();

        // send email
        sendPasswordResetEmail(account, origin);
    }

    public void ValidateResetToken(ValidateResetTokenRequest model) {
        getAccountByResetToken(model.Token);
    }

    public void ResetPassword(ResetPasswordRequest model) {
        var account = getAccountByResetToken(model.Token);

        // update password and remove reset token
        account.PasswordHash = BCrypt.HashPassword(model.Password);
        account.PasswordReset = DateTime.UtcNow;
        account.ResetToken = null;
        account.ResetTokenExpires = null;

        _context.Accounts.Update(account);
        _context.SaveChanges();
    }

    public IEnumerable<AccountResponse> GetAll() {
        var accounts = _context.Accounts;
        return _mapper.Map<IList<AccountResponse>>(accounts);
    }

    public AccountResponse GetById(int id) {
        var account = getAccount(id);
        return _mapper.Map<AccountResponse>(account);
    }

    public AccountResponse Create(CreateRequest model) {
        // validate
        if (_context.Accounts.Any(x => x.Email == model.Email))
            throw new AppException($"Email adresi '{model.Email}' sistemde mevcut!");

        // map model to new account object
        var account = _mapper.Map<Account>(model);
        account.Created = DateTime.UtcNow;
        account.Verified = DateTime.UtcNow;

        // hash password
        account.PasswordHash = BCrypt.HashPassword(model.Password);

        // save account
        _context.Accounts.Add(account);
        _context.SaveChanges();

        return _mapper.Map<AccountResponse>(account);
    }

    public AccountResponse Update(int id, UpdateRequest model) {
        var account = getAccount(id);

        // validate
        if (account.Email != model.Email && _context.Accounts.Any(x => x.Email == model.Email))
            throw new AppException($"Email adresi '{model.Email}' sistemde mevcut!");

        // hash password if it was entered
        if (!string.IsNullOrEmpty(model.Password))
            account.PasswordHash = BCrypt.HashPassword(model.Password);

        // copy model to account and save
        _mapper.Map(model, account);
        account.Updated = DateTime.UtcNow;
        _context.Accounts.Update(account);
        _context.SaveChanges();

        return _mapper.Map<AccountResponse>(account);
    }

    public void Delete(int id) {
        var account = getAccount(id);

        account.IsDisabled = true;
        account.PasswordHash = BCrypt.HashPassword(Guid.NewGuid().ToString());

        _context.Update(account);
        _context.SaveChanges();
    }

    //? HELPER METHODS
    public Account getAccount(int id) {
        var account = _context.Accounts.Find(id);
        if (account == null) throw new KeyNotFoundException("Kullanıcı bulunamadı");
        return account;
    }

    public Account getAccountByRefreshToken(string token) {
        var account = _context.Accounts.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));

        if (account == null)
            throw new AppException("Geçersiz token");

        return account;
    }

    public Account getAccountByResetToken(string token) {
        var account = _context.Accounts.SingleOrDefault(x =>
            x.ResetToken == token && x.ResetTokenExpires > DateTime.UtcNow);
        if (account == null) throw new AppException("Yenileme süresi geçmiş.");
        return account;
    }

    // public string generateJwtToken(Account account) {
    //     // generate token that is valid for 7 days
    //     var tokenHandler = new JwtSecurityTokenHandler();
    //     var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
    //     var tokenDescriptor = new SecurityTokenDescriptor {
    //         Subject = new ClaimsIdentity(new[] { new Claim("id", account.id.ToString()) }),
    //         Expires = DateTime.UtcNow.AddMinutes(15),
    //         SigningCredentials = new SigningCredentials(
    //             new SymmetricSecurityKey(key),
    //             SecurityAlgorithms.HmacSha256Signature)
    //     };
    //     var token = tokenHandler.CreateToken(tokenDescriptor);
    //     return tokenHandler.WriteToken(token);
    // }

    //* jwtUtils.cs deki getUniqueTokenGuid ile değiştirildi.
    // public static string generateResetToken() {
    //     // token is a cryptographically strong random sequence of values
    //     var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

    //     // ensure token is unique by checking against db
    //     var tokenIsUnique = !_context.Accounts.Any(x => x.ResetToken == token);
    //     if (tokenIsUnique)
    //         return generateResetToken();

    //     return token;
    // }

    //* jwtUtils.cs deki getUniqueTokenGuid ile değiştirildi. 
    // public static string generateVerificationToken() {
    //     // token is a cryptographically strong random sequence of values
    //     var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

    //     // db'ye karşı kontrol ederek token benzersiz olduğundan emin olun
    //     var tokenIsUnique = !_context.Accounts.Any(x => x.VerificationToken == token);
    //     if (!tokenIsUnique)
    //         return generateVerificationToken();

    //     return token;
    // }

    public RefreshToken rotateRefreshToken(RefreshToken refreshToken, string ipAddress) {
        var newRefreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);
        revokeRefreshToken(refreshToken, ipAddress, "Yeni token ile değiştirildi", newRefreshToken.Token);
        return newRefreshToken;
    }

    public void removeOldRefreshTokens(Account account) {
        // remove old inactive refresh tokens from user based on TTL in app settings
        account.RefreshTokens.RemoveAll(x =>
            !x.IsActive &&
            x.Created.AddDays(_appSettings.RefreshTokenTTL) <= DateTime.UtcNow);
    }

    public void revokeDescendantRefreshTokens(RefreshToken refreshToken, Account account, string ipAddress, string reason) {
        // recursively traverse the refresh token chain and ensure all descendants are revoked
        if (!string.IsNullOrEmpty(refreshToken.ReplacedByToken)) {
            var childToken = account.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken.ReplacedByToken);
            if (childToken.IsActive)
                revokeRefreshToken(childToken, ipAddress, reason);
            else
                revokeDescendantRefreshTokens(childToken, account, ipAddress, reason);
        }
    }

    public void revokeRefreshToken(RefreshToken token, string ipAddress, string reason = null, string replacedByToken = null) {
        token.Revoked = DateTime.UtcNow;
        token.RevokedByIp = ipAddress;
        token.ReasonRevoked = reason;
        token.ReplacedByToken = replacedByToken;
    }

    public void sendVerificationEmail(Account account, string origin) {
        string message;
        if (!string.IsNullOrEmpty(origin)) {
            // origin exists if request sent from browser single page app (e.g. Angular or React)
            // so send link to verify via single page app
            var verifyUrl = $"{origin}/verify-email?token={account.VerificationToken}";
            message = $@"<p>E-posta adresinizi doğrulamak için lütfen aşağıdaki bağlantıya tıklayın:</p>
                            <p><a href=""{verifyUrl}"">{verifyUrl}</a></p>";
        } else {
            // origin missing if request sent directly to api (e.g. from Postman)
            // so send instructions to verify directly with api
            message = $@"<p>Lütfen e-posta adresinizi doğrulamak için aşağıdaki api ile kodu kullanın <code>/accounts/verify-email</code> </p>
                            <p><code>{account.VerificationToken}</code></p>";
        }

        _emailService.Send(
            to: account.Email,
            subject: "Kayıt Doğrulama - E-postayı Doğrulayın",
            html: $@"<h4>E-posta doğrula</h4> <p>Kayıt olduğunuz için teşekkürler!</p>
                {message}
                <p>Bu işlem bilginiz dahilinde değilse, Bu emaili dikkate almayınız.</p>"
        );
    }

    public void sendAlreadyRegisteredEmail(string email, string origin) {
        string message;
        if (!string.IsNullOrEmpty(origin))
            message = $@"<p>Şifrenizi bilmiyorsanız lütfen <a href=""{origin}/forgot-password"">şifremi unuttum</a> sayfasını ziyaret edin.</p>";
        else
            message = "<p>Şifrenizi bilmiyorsanız, şifrenizi sistem yöneticisi ile görüşüp değiştirilmesini sağlayabilirsiniz.</p>";

        _emailService.Send(
            to: email,
            subject: "Kayıt Doğrulama - Kayıtlı E-posta hesabı",
            html: $@"<h4>Kayıtlı E-posta hesabı</h4>
                        <p>E-postanız <strong>{email}</strong> daha önce kayıt edilmiş.</p>
                        {message}
                        <p>Bu işlem bilginiz dahilinde değilse, Bu emaili dikkate almayınız.</p>"
        );
    }

    public void sendPasswordResetEmail(Account account, string origin) {
        string message;
        if (!string.IsNullOrEmpty(origin)) {
            var resetUrl = $"{origin}/reset-password?token={account.ResetToken}";
            message = $@"<p>Lütfen şifrenizi sıfırlamak için aşağıdaki linke tıklayınız, link 1 gün süreyle geçerli olacaktır:</p>
                            <p><a href=""{resetUrl}"">{resetUrl}</a></p>";
        } else {
            message = $@"<p>Şifrenizi sıfırlamak için lütfen aşağıdaki kodu sistem yöneticinize bildiriniz.</p>
                            <p><code>{account.ResetToken}</code></p>";
        }

        _emailService.Send(
            to: account.Email,
            subject: "Şifre sıfırlama",
            html: $@"<h4>Şifrenizi sıfırlayın</h4>
                        {message}
                        <p>Bir şifre sıfırlama isteğinde bulunmadıysanız, Bu emaili dikkate almayınız.</p>"
        );
    }
}
