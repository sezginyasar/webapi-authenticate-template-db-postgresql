using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapiV2.Entities;

[Table("account")]
public class Account {
    [Key, Column("id")]
    public int id { get; set; }

    [Column("adi"), StringLength(255)]
    public string Adi { get; set; }

    [Column("soyadi"), StringLength(255)]
    public string Soyadi { get; set; }

    [Column("email"), StringLength(500)]
    public string Email { get; set; }

    //! daha sonra silinecek
    [Column("password")]
    public string? Password { get; set; }

    [Column("password_hash")]
    public string PasswordHash { get; set; }

    [Column("accept_terms")]
    public bool AcceptTerms { get; set; }

    [Column("role")]
    public Role Role { get; set; }

    [Column("verification_token")]
    public string? VerificationToken { get; set; }

    [Column("verified")]
    public DateTime? Verified { get; set; }

    [Column("is_verified")]
    public bool IsVerified => Verified.HasValue || PasswordReset.HasValue;

    [Column("reset_token")]
    public string? ResetToken { get; set; }

    [Column("reset_token_expires")]
    public DateTime? ResetTokenExpires { get; set; }

    [Column("password_reset")]
    public DateTime? PasswordReset { get; set; }

    [Column("created")]
    public DateTime Created { get; set; }

    [Column("updated")]
    public DateTime? Updated { get; set; }

    [Column("refresh_tokens")]
    public List<RefreshToken>? RefreshTokens { get; set; }

    [Column("is_disabled")]
    public bool IsDisabled { get; set; }

    public bool OwnsToken(string token) {
        return this.RefreshTokens?.Find(x => x.Token == token) != null;
    }
}