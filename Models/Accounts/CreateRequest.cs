namespace webapiV2.Models.Accounts;

using System.ComponentModel.DataAnnotations;
using webapiV2.Entities;

public class CreateRequest {
    [Required]
    public string Adi { get; set; }

    [Required]
    public string Soyadi { get; set; }

    [Required]
    [EnumDataType(typeof(Role))]
    public string Role { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }

    [Required]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }
}