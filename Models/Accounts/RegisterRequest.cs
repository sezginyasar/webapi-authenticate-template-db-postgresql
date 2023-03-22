namespace webapiV2.Models.Accounts;

using System.ComponentModel.DataAnnotations;
using webapiV2.Entities;

public class RegisterRequest {
    [Required]
    public string Adi { get; set; } = null!;

    [Required]
    public string Soyadi { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }

    [Required]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }

    [Range(typeof(bool), "true", "true")]
    public bool AcceptTerms { get; set; }
}