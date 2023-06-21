namespace webapiV2.Models.Menus;

public class MenuResponse {
    public int Id { get; set; }
    public int ParentId { get; set; }
    public string? Menuler { get; set; }
    public string? Icon { get; set; }
    public string PageName { get; set; } = null!;
    public string Title { get; set; } = null!;
    public bool SubMenu { get; set; }
    public short Order { get; set; }
    public short Status { get; set; }
}