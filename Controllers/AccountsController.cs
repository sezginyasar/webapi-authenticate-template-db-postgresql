namespace webapiV2.Controllers;

using Microsoft.AspNetCore.Mvc;
using webapiV2.Authorization;
using webapiV2.Entities;
using webapiV2.Models.Accounts;
using webapiV2.Services.Accounts;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class AccountsController : BaseController {
    private readonly IAccountService _accountService;

    public AccountsController(IAccountService accountService) {
        _accountService = accountService;
    }

    [AllowAnonymous]
    [HttpPost("authenticate")]
    public ActionResult<AuthenticateResponse> Authenticate(AuthenticateRequest model) {
        var response = _accountService.Authenticate(model, ipAddress());
        setTokenCookie(response.RefreshToken);
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public ActionResult<AuthenticateResponse> RefreshToken() {
        var refreshToken = Request.Cookies["refreshToken"];
        var response = _accountService.RefreshToken(refreshToken, ipAddress());
        setTokenCookie(response.RefreshToken);
        //Console.WriteLine("refresh token cookie", response.RefreshToken);
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("revoke-token")]
    public IActionResult RevokeToken(RevokeTokenRequest model) {
        // kullanıcıyı kapatmak sistemden atmak için yada 
        // kullanıcının kendi açık hesaplarını kapatması için kullanılır
        // accept refresh token in request body or cookie
        var token = model.Token ?? Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(token))
            return BadRequest(new { message = "Geçersiz Token" });

        // users can revoke their own tokens and admins can revoke any tokens
        // iptal edilen tokenı iptal eden kişi ADMIN rolündeyse herkesinkini iptal edebilir. role USER ise sadece kendisinin token larını iptal edebilir.
        // OwnsToken gelen token bilgisini kendi refresh tokenların arasında varmı diye kontrol ediyor.
        //bool OwnsToken = _accountService.OwnsToken(token, Account.Id); //

        if (!Account.OwnsToken(token) && Account.Role != Role.Admin)
            return Unauthorized(new { message = "Unauthorized" });
        
        _accountService.RevokeToken(token, ipAddress());
        deleteTokenCookie("refreshToken");

        return Ok(new { message = "Token iptal edildi." });
    }

    //! daha sonra AllowAnonymous kaldırılacak. Bizim sistemlerde kayıtlı bir kullanıcı hesap oluşturabilir.
    [AllowAnonymous]
    [HttpPost("register")]
    public IActionResult Register(RegisterRequest model) {
        _accountService.Register(model, Request.Headers["origin"]);
        return Ok(new { message = "Kayıt Başarılı, lütfen doğrulama talimatları için e-postanızı kontrol edin" });
    }

    [AllowAnonymous]
    [HttpPost("verify-email")]
    public IActionResult VerifyEmail(VerifyEmailRequest model) {
        _accountService.VerifyEmail(model.Token);
        return Ok(new { message = "Doğrulama başarılı, şimdi giriş yapabilirsiniz" });
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public IActionResult ForgotPassword(ForgotPasswordRequest model) {
        _accountService.ForgotPassword(model, Request.Headers["origin"]);
        return Ok(new { message = "Parola sıfırlama talimatları için lütfen e-postanızı kontrol edin" });
    }

    [AllowAnonymous]
    [HttpPost("validate-reset-token")]
    public IActionResult ValidateResetToken(ValidateResetTokenRequest model) {
        _accountService.ValidateResetToken(model);
        return Ok(new { message = "Geçerli token" });
    }

    //TODO Login olmuş kullanıcının şifre yenilemesi için ayrıca bir işelm yapılacak
    [AllowAnonymous]
    [HttpPost("reset-password")]
    public IActionResult ResetPassword(ResetPasswordRequest model) {
        _accountService.ResetPassword(model);
        return Ok(new { message = "Şifre sıfırlama başarılı, şimdi giriş yapabilirsiniz" });
    }

    [Authorize(Role.Admin)]
    [HttpGet]
    public ActionResult<IEnumerable<AccountResponse>> GetAll() {
        var accounts = _accountService.GetAll();
        return Ok(accounts);
    }

    [HttpGet("{id:int}")]
    public ActionResult<AccountResponse> GetById(int id) {
        // users can get their own account and admins can get any account
        if (id != Account.id && Account.Role != Role.Admin)
            return Unauthorized(new { message = "Unauthorized" });

        var account = _accountService.GetById(id);
        return Ok(account);
    }

    [Authorize(Role.Admin)]
    [HttpPost]
    public ActionResult<AccountResponse> Create(CreateRequest model) {
        var account = _accountService.Create(model);
        return Ok(account);
    }

    [HttpPut("{id:int}")]
    public ActionResult<AccountResponse> Update(int id, UpdateRequest model) {
        // users can update their own account and admins can update any account
        if (id != Account.id && Account.Role != Role.Admin)
            return Unauthorized(new { message = "Unauthorized" });

        // only admins can update role
        if (Account.Role != Role.Admin)
            model.Role = null;

        var account = _accountService.Update(id, model);
        return Ok(account);

        // _userService.Update(id, model);
        // return Ok(new { message = "Kullanıcı güncelleme başarılı" });
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id) {

        // users can delete their own account and admins can delete any account
        if (id != Account.id && Account.Role != Role.Admin)
            return Unauthorized(new { message = "Unauthorized" });

        _accountService.Delete(id);
        return Ok(new { message = "Kullanıcı pasif duruma getirildi." });
    }

    // helper methods
    private void setTokenCookie(string token) {
        // append cookie with refresh token to the http response
        var cookieOptions = new CookieOptions {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(5),
            //SameSite = SameSiteMode.Lax,
        };
        Response.Cookies.Append("refreshToken", token, cookieOptions);
    }

    private void deleteTokenCookie(string tokenName){
        Response.Cookies.Delete(tokenName);
    }

    private string ipAddress() {
        // get source ip address for the current request
        if (Request.Headers.ContainsKey("X-Forwarded-For"))
            return Request.Headers["X-Forwarded-For"];
        else
            return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
    }
}