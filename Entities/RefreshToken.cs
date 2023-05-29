namespace webapiV2.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

[Table("refresh_token")]
[Owned]
public class RefreshToken {
    [Key, Column("id")]
    public int Id { get; set; }

    [Column("account_id")]
    public int AccountId { get; set; }

    [ForeignKey(nameof(AccountId))]
    public Account Account { get; set; }

    [Column("token")]
    public string Token { get; set; }

    [Column("expires")]
    public DateTime Expires { get; set; }

    [Column("created")]
    public DateTime Created { get; set; }

    [Column("created_by_ip")]
    public string CreatedByIp { get; set; }

    [Column("revoked")]
    public DateTime? Revoked { get; set; }

    [Column("revoked_by_ip")]
    public string? RevokedByIp { get; set; }

    [Column("resplaced_by_token")]
    public string? ReplacedByToken { get; set; }

    [Column("reason_revoked")]
    public string? ReasonRevoked { get; set; }

    [Column("is_expired")]
    public bool IsExpired => DateTime.UtcNow >= Expires;

    [Column("is_revoked")]
    public bool IsRevoked => Revoked != null;

    [Column("is_active")]
    public bool IsActive => !IsRevoked && !IsExpired;
}