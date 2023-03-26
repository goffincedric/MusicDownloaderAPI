using MusicDownloader.Shared.Constants;
using MusicDownloader.Shared.Options._base;

namespace MusicDownloader.Shared.Options;

public class JwtOptions : SettingsBase
{
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string Secret { get; set; }
    public int Expiration { get; set; }

    public override void SetFromEnvironmentVariables()
    {
        // Jwt settings
        Issuer = GetValue<string>(ApplicationConstants.EnvKeys.JwtIssuer);
        Audience = GetValue<string>(ApplicationConstants.EnvKeys.JwtAudience);
        Secret = GetValue<string>(ApplicationConstants.EnvKeys.JwtSecret);
        Expiration = GetValue<int>(ApplicationConstants.EnvKeys.JwtExpiration);
    }
}