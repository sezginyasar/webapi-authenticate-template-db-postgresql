namespace webapiV2.Models.Accounts;

using System.ComponentModel.DataAnnotations;

public class ValidateResetTokenRequest
{
    [Required]
    public string Token { get; set; }
}