namespace JWTAuth.Core;

// https://itecnote.com/tecnote/c-how-to-map-config-in-iconfigurationsection-to-a-simple-class/
public class AppSettingsOptions
{
    public const string AppSettings = "AppSettings";

    public string Secret { get; set; } = String.Empty;

    public JwtOption? Jwt { get; set; }
}

// ReSharper disable once ClassNeverInstantiated.Global
public class JwtOption
{
    public string Audience { get; set; } = String.Empty;
    public string Issuer { get; set; } = String.Empty;
    
    public string SecretKey { get; set; } = String.Empty;
}