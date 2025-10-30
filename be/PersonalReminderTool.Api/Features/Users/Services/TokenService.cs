using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using PersonalReminderTool.Api.Data.Entities;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace PersonalReminderTool.Api.Features.Users.Services;

internal sealed class TokenService(IConfiguration configuration)
{
    public string GenerateJwt(User user)
    {
        string secretKey = configuration["Jwt:Secret"]!;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        int expiration = configuration.GetValue<int>("Jwt:Expiration");
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptior = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            ]),
            Expires = DateTime.UtcNow.AddMinutes(expiration),
            Issuer = configuration["Jwt:Issuer"],
            Audience = configuration["Jwt:Audience"],
            SigningCredentials = credentials
        };

        var handler = new JsonWebTokenHandler();

        string token = handler.CreateToken(tokenDescriptior);

        return token;
    }

    public RefreshToken CreateRefreshToken(string userId) => new RefreshToken
    {
        Id = Guid.NewGuid().ToString(),
        Token = GenerateRefreshTokenString(),
        UtcExpiry = DateTime.UtcNow.AddDays(14),
        UserId = userId
    };

    public string GenerateRefreshTokenString() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

    public static CookieOptions CreateSecureCookieOptions() => new()
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.None,
        Expires = DateTime.UtcNow.AddDays(14)
    };
}
