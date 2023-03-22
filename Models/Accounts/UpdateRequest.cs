namespace webapiV2.Models.Accounts;

using System.ComponentModel.DataAnnotations;
using webapiV2.Entities;

public class UpdateRequest {
    private string _password;
    private string _confirmPassword;
    private string _role;
    private string _email;

    public string Adi { get; set; }
    public string Soyadi { get; set; }

    [EnumDataType(typeof(Role))]
    public string Role {
        get => _role;
        set => _role = replaceEmptyWithNull(value);
    }

    [EmailAddress]
    public string Email {
        get => _email;
        set => _email = replaceEmptyWithNull(value);
    }

    [MinLength(6)]
    public string Password {
        get => _password;
        set => _password = replaceEmptyWithNull(value);
    }

    [Compare("Password")]
    public string ConfirmPassword {
        get => _confirmPassword;
        set => _confirmPassword = replaceEmptyWithNull(value);
    }

    // helpers
    private string replaceEmptyWithNull(string value) {
        // alanı isteğe bağlı yapmak için boş dizeyi null ile değiştirin
        return string.IsNullOrEmpty(value) ? null : value;
    }
}