using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Shortly.Configuration.Options;
using Shortly.Domain.Identity;
using Shortly.Domain.Services;

namespace Shortly.Infrastructure.Security;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _options;
    private readonly SymmetricSecurityKey _secretKey;

    public JwtTokenService(
        IOptions<JwtOptions> options,
        ISecretDerivationService secrets)
    {
        _options = options.Value;

        var secretBytes = secrets.GetJwtSigningKey();

        _secretKey = new SymmetricSecurityKey(secretBytes);
    }

    public string CreateAccessToken(
        AppUser user,
        IEnumerable<string>? roles = null)
    {
        var credentials = new SigningCredentials(_secretKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? user.Email ?? user.Id.ToString()),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString())
        };

        if (roles != null)
        {
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        }

        var now = DateTime.UtcNow;

        var token = new JwtSecurityToken(
           issuer: _options.Issuer,
           audience: _options.Audience,
           claims: claims,
           notBefore: now,
           expires: now.AddMinutes(_options.AccessTokenMinutes),
           signingCredentials: credentials);

        var handler = new JwtSecurityTokenHandler();

        return handler.WriteToken(token);
    }
}
