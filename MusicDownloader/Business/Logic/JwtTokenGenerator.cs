﻿using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MusicDownloader.Pocos.User;
using MusicDownloader.Shared.Options;
using ILogger = Serilog.ILogger;

namespace MusicDownloader.Business.Logic;

public class JwtTokenGenerator
{
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger _logger;

    public JwtTokenGenerator(IOptions<JwtOptions> jwtOptions, ILogger logger)
    {
        _jwtOptions = jwtOptions.Value;
        _logger = logger;
    }

    public string CreateToken(ApiUser user)
    {
        var expiration = DateTime.UtcNow.AddMinutes(_jwtOptions.Expiration);
        var token = CreateJwtToken(
            CreateClaims(user),
            CreateSigningCredentials(),
            expiration
        );
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    private JwtSecurityToken CreateJwtToken(
        IEnumerable<Claim> claims, SigningCredentials credentials, DateTime expiration
    ) => new(
        _jwtOptions.Issuer,
        _jwtOptions.Audience,
        claims,
        expires: expiration,
        signingCredentials: credentials
    );

    private IEnumerable<Claim> CreateClaims(ApiUser user)
    {
        try
        {
            return new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Uuid),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)),
                new(JwtRegisteredClaimNames.Name, user.Name)
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "Couldn't generate jwt token claims for user {0}", user.Uuid);
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