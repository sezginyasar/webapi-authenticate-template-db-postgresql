using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapiV2.Entities.PageAuthorization;

[Table("user_page_permissions")]
public class UserPagePermissions  {
    [Key, Column("id")]
    public int Id { get; set; }

    [Column("page_id")]
    public int PagesId { get; set; }

    [ForeignKey(nameof(PagesId))]
    public Pages Pages { get; set; } = null!;

    [Column("user_id")]
    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public Account? Account { get; set; }

    [Column("read")]
    public bool Read { get; set; } = false;

    [Column("save")]
    public bool Save { get; set; } = false;

    [Column("Update")]
    public bool Update { get; set; } = false;

    [Column("Delete")]
    public bool Delete { get; set; } = false;

    [Column("additional_auth_one")]
    public bool AdditionalAuthOne { get; set; } = false;

    [Column("additional_auth_two")]
    public bool AdditionalAuthTwo { get; set; } = false;

    [Column("additional_auth_three")]
    public bool AdditionalAuthThree { get; set; } = false;

    [Column("additional_auth_four")]
    public bool AdditionalAuthFour { get; set; } = false;

    [Column("additional_auth_five")]
    public bool AdditionalAuthFive { get; set; } = false;
}