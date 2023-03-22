using webapiV2.Entities;

namespace webapiV2.Models.Accounts;

public class AccountResponse {
    public int id { get; set; }
    public string Adi { get; set; }
    public string Soyadi { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
    public bool IsVerified { get; set; }
}