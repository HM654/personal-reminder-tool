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

    public RefreshToken CreateRefreshTokenRecord(User user) => new RefreshToken
    {
        Id = Guid.NewGuid().ToString(),
        Token = GenerateRefreshToken(),
        UtcExpiry = DateTime.UtcNow.AddDays(14),
        UserId = user.Id
    };

    public string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

    public static CookieOptions CreateSecureCookieOptions() => new()
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.None,
        Expires = DateTime.UtcNow.AddDays(14)
    };
}
