using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapiV2.Entities.PageAuthorization;

[Table("pages")]
public class Pages {
    [Key, Column("id")]
    public int Id { get; set; }

    [Column("parent_id")]
    public int ParentId { get; set; }

    [Column("page_name"), StringLength(100)]
    public string PageName { get; set; } = null!;

    [Column("page_group_name"), StringLength(100)]
    public string PageGroupName { get; set; } = null!;

    [Column("path"), StringLength(100)]
    public string Path { get; set; } = null!;

    [Column("icon"), StringLength(50)]
    public string? Icon { get; set; }

    [Column("order", TypeName = "smallint")]
    public short Order { get; set; }

    [Column("additional_auth_one"), StringLength(255)]
    public string? AdditionalAuthOne { get; set; }

    [Column("additional_auth_two"), StringLength(255)]
    public string? AdditionalAuthTwo { get; set; }

    [Column("additional_auth_three"), StringLength(255)]
    public string? AdditionalAuthThree { get; set; }

    [Column("additional_auth_four"), StringLength(255)]
    public string? AdditionalAuthFour { get; set; }

    [Column("additional_auth_five"), StringLength(255)]
    public string? AdditionalAuthFive { get; set; }

    [Column("status", TypeName = "smallint")]
    public short Status { get; set; }

    public List<UserPagePermissions>? UserPagePermissions { get; set; }
}