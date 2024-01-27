using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MusicDownloader.Pocos.User;
using MusicDownloader.Shared.Options;
using ILogger = Serilog.ILogger;

namespace MusicDownloader.Business.Logic;

public class JwtTokenGenerator(IOptions<JwtOptions> jwtOptions, ILogger logger)
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public string CreateToken(ApiUser user)
    {
        var expiration = DateTimeOffset.UtcNow.AddMinutes(_jwtOptions.Expiration);
        var token = CreateJwtToken(
            CreateClaims(user),
            CreateSigningCredentials(),
            expiration
        );
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    private JwtSecurityToken CreateJwtToken(
        List<Claim> claims, SigningCredentials credentials, DateTimeOffset expiration
    ) => new(
        _jwtOptions.Issuer,
        _jwtOptions.Audience,
        claims,
        expires: expiration.UtcDateTime,
        signingCredentials: credentials
    );

    private List<Claim> CreateClaims(ApiUser user)
    {
        try
        {
            return
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Uuid),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.Name)
            ];
        }
        catch (Exception e)
        {
            logger.Error(e, "Couldn't generate jwt token claims for user {0}", user.Uuid);
            throw;
        }
    }

    private SigningCredentials CreateSigningCredentials()
    {
        return new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtOptions.Secret)
            ),
            SecurityAlgorithms.HmacSha256
        );
    }
}