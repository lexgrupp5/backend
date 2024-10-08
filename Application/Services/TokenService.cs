using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Interfaces;
using Domain.Configuration;
using Domain.Constants;
using Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services;

public class TokenService(TokenConfig tokenConfig) : ITokenService
{
    private readonly TokenConfig _tc = tokenConfig;

    public string GenerateAccessToken(User user, ICollection<string>? roles)
    {
        var credentials = CreateSigningCredentials(_tc.Access.Secret);
        var claims = CreateClaims(user, roles);
        var tokenOptions = CreateTokenOptions(_tc.Access, credentials, claims);
        var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

        return accessToken;
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
        }
        var token = Convert.ToBase64String(randomNumber);
        return token;
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token, string secret)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(
            token,
            tokenValidationParameters,
            out SecurityToken securityToken
        );
        var isValid =
            securityToken is JwtSecurityToken jwtSecurityToken
            && jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase
            );

        return isValid ? principal : throw new SecurityTokenException("Invalid token");
    }

    private static JwtSecurityToken CreateTokenOptions(
        AccessConfig options,
        SigningCredentials credentials,
        IEnumerable<Claim> claims
    ) =>
        new(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(options.ExpirationInMinutes),
            signingCredentials: credentials
        );

    private static List<Claim> CreateClaims(User user, ICollection<string>? roles)
    {
        var claims = new List<Claim>()
        {  
            new(ClaimTypes.Name, user.UserName!),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };

        claims.AddRange(roles?.Select(r => new Claim(ClaimTypes.Role, r)) ?? []);

        return claims;
    }

    private static SigningCredentials CreateSigningCredentials(string key) =>
        new(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256);
}
