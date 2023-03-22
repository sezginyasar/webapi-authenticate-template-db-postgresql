namespace webapiV2.Entities;

public class Account {
    public int id { get; set; } 
    public string Adi { get; set; }
    public string Soyadi { get; set; }
    public string Email { get; set; }
    //! daha sonra silinecek
    public string? Password { get; set; }
    public string PasswordHash { get; set; }
    public bool AcceptTerms { get; set; }
    public Role Role { get; set; }
    public string? VerificationToken { get; set; }
    public DateTime? Verified { get; set; }
    public bool IsVerified => Verified.HasValue || PasswordReset.HasValue;
    public string? ResetToken { get; set; }
    public DateTime? ResetTokenExpires { get; set; }
    public DateTime? PasswordReset { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
    public List<RefreshToken>? RefreshTokens { get; set; }
    public bool IsDisabled { get; set; }

    public bool OwnsToken(string token) 
    {
        return this.RefreshTokens?.Find(x => x.Token == token) != null;
    }
}