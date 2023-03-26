namespace MusicDownloader.Shared.Constants;

public static class ApplicationConstants
{
    public static readonly string FFMpegPath = Path.Combine("binaries", "FFMpeg");

    public static class Cors
    {
        public const string AllowAllOrigins = "AllowAllOrigins";
    }
    
    public static class Jwt
    {
        public const string AuthenticationScheme = "JWT";
        public const string HeaderName = "Authorization";
        public const string BearerPrefix = "Bearer";
    }

    public static class EnvKeys
    {
        public const string ApiUsers = "API_USERS";

        public const string JwtIssuer = "JWT_ISSUER";
        public const string JwtAudience = "JWT_AUDIENCE";
        public const string JwtSecret = "JWT_SECRET";
        public const string JwtExpiration = "JWT_EXPIRATION";
    }
}