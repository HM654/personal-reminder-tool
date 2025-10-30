using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using PersonalReminderTool.Api.Data.Entities;
using PersonalReminderTool.Api.Features.Users.Services;

namespace PersonalReminderTool.Tests;

public sealed class TokenServiceTests
{
    private readonly TokenService _tokenService;

    public TokenServiceTests()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Secret"] = "SecretJwtKey01234567890123456789",
                ["Jwt:Issuer"] = "TestIssuer",
                ["Jwt:Audience"] = "TestAudience",
                ["Jwt:Expiration"] = "60"
            })
            .Build();

        _tokenService = new TokenService(configuration);
    }

    [Fact]
    public void GenerateJwt_ReturnsValidToken()
    {
        // Arrange
        var user = new User
        {
            Id = "user1",
            Email = "test@example.com",
            Password = ""
        };

        // Act
        var token = _tokenService.GenerateJwt(user);
        var handler = new JsonWebTokenHandler();
        var jwt = handler.ReadJsonWebToken(token);

        // Assert
        Assert.NotEmpty(token);
        Assert.Equal(user.Id, jwt.Subject);
        Assert.Equal(user.Email, jwt.GetClaim("email").Value);
        Assert.True(jwt.IssuedAt <= DateTime.UtcNow);
        Assert.True(jwt.ValidTo > DateTime.UtcNow);
        Assert.True(jwt.ValidTo <= DateTime.UtcNow.AddMinutes(60));
    }

    [Fact]
    public void CreateRefreshToken_ReturnsValidRefreshToken()
    {
        // Arrange
        var userId = "TestUserId";

        // Act
        var refreshToken = _tokenService.CreateRefreshToken(userId);

        // Assert
        Assert.NotEmpty(refreshToken.Id);
        Assert.NotEmpty(refreshToken.Token);
        Assert.Equal(userId, refreshToken.UserId);
        Assert.True(refreshToken.UtcExpiry > DateTime.UtcNow);
        Assert.True(refreshToken.UtcExpiry <= DateTime.UtcNow.AddDays(14));
    }

    [Fact]
    public void GenerateRefreshTokenString_ReturnsUniqueTokens()
    {
        // Act
        var token1 = _tokenService.GenerateRefreshTokenString();
        var token2 = _tokenService.GenerateRefreshTokenString();

        // Assert
        Assert.NotEqual(token1, token2);
    }

    [Fact]
    public void CreateSecureCookieOptions_ReturnsCorrectOptions()
    {
        // Act
        var options = TokenService.CreateSecureCookieOptions();

        // Assert
        Assert.True(options.HttpOnly);
        Assert.True(options.Secure);
        Assert.Equal(SameSiteMode.None, options.SameSite);
        Assert.True(options.Expires.HasValue);
        Assert.True(options.Expires.Value <= DateTimeOffset.UtcNow.AddDays(14));
    }
}
