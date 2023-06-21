using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapiV2.Entities.Menus;

[Table("menu")]
public class Menu {
    [Key, Column("id")]
    public int Id { get; set; }

    [Column("parent_id")]
    public int ParentId { get; set; }

    [Column("menuler", TypeName = "jsonb")]
    public string? Menuler { get; set; }

    [Column("icon"), StringLength(255)]
    public string? Icon { get; set; }

    [Column("page_name"), StringLength(255)]
    public string PageName { get; set; } = null!;

    [Column("title"), StringLength(255)]
    public string Title { get; set; } = null!;

    [Column("submenu"), StringLength(100)]
    public bool SubMenu { get; set; }

    [Column("order", TypeName = "smallint")]
    public short Order { get; set; }

    [Column("status", TypeName = "smallint")]
    public short Status { get; set; }
}