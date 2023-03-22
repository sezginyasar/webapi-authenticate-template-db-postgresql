namespace webapiV2.Models.Accounts;

using System.Text.Json.Serialization;
using webapiV2.Entities;

public class AuthenticateResponse {
    public int id { get; set; }
    public string Adi { get; set; }
    public string Soyadi { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
    public bool IsVerified { get; set; }
    public string JwtToken { get; set; }

    //! JsonIgnore daha testler bittikten sonra açılacak.
    [JsonIgnore] // refresh token cookie için döndürülüyor
    public string RefreshToken { get; set; }
}