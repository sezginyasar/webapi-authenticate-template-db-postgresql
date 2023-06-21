using System.ComponentModel.DataAnnotations.Schema;

namespace webapiV2.Models.Pages;

public class PageUpdateRequest {
    public int ParentId { get; set; }
    public string PageName { get; set; } = null!;
    public string PageGroupName { get; set; } = null!;
    public string Path { get; set; } = null!;
    public string? Icon { get; set; }
    public short Order { get; set; }
    public string? AdditionalAuthOne { get; set; }
    public string? AdditionalAuthTwo { get; set; }
    public string? AdditionalAuthThree { get; set; }
    public string? AdditionalAuthFour { get; set; }
    public string? AdditionalAuthFive { get; set; }
    public short Status { get; set; }
}