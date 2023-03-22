namespace webapiV2.Helpers;

public class AppSettings
{
    //! Helpers klasörü altında JwtUtls.cs ve JwtMiddleware.cs dosyaları silinince JwtKey objeside silinecek.
    //public string JwtKey { get; set; }= null!;

     public string Secret { get; set; }

    // refresh token time to live (in days), inactive tokens are
    // automatically deleted from the database after this time
    public int RefreshTokenTTL { get; set; }
    
    public string EmailFrom { get; set; }
    public string SmtpHost { get; set; }
    public int SmtpPort { get; set; }
    public string SmtpUser { get; set; }
    public string SmtpPass { get; set; }
}