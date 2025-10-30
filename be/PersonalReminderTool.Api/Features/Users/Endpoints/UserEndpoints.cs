using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalReminderTool.Api.Data;
using PersonalReminderTool.Api.Data.Entities;
using PersonalReminderTool.Api.Features.Users.Dtos;
using PersonalReminderTool.Api.Features.Users.Services;

namespace PersonalReminderTool.Api.Features.Users.Endpoints;

internal static class UserEndpoints
{
    public static void MapEndpoints(this RouteGroupBuilder api)
    {
        api.MapPost("/login", LoginAsync);

        api.MapPost("/refresh", RefreshAsync);
    }

    private static async Task<IResult> LoginAsync([FromBody] LoginRequestDto request, HttpContext httpContext, ApplicationDbContext context, TokenService tokenService, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            return Results.BadRequest("Email and password must be provided.");

        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user is null)
            return Results.BadRequest("User does not exist.");

        var passwordHasher = new PasswordHasher<User>();
        var result = passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);

        if (result == PasswordVerificationResult.Failed)
            return Results.Unauthorized();

        var jwt = tokenService.GenerateJwt(user);
        var refreshTokenRecord = tokenService.CreateRefreshToken(user.Id);

        await context.RefreshTokens.AddAsync(refreshTokenRecord, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        httpContext.Response.Cookies.Append("refreshToken", refreshTokenRecord.Token, TokenService.CreateSecureCookieOptions());

        return Results.Ok(jwt);
    }

    private static async Task<IResult> RefreshAsync(HttpContext httpContext, ApplicationDbContext context, TokenService tokenService, CancellationToken cancellationToken)
    {
        if (!httpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshTokenSent))
            return Results.Unauthorized();
        
        var refreshToken = await context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshTokenSent, cancellationToken);

        if (refreshToken is null || refreshToken.UtcExpiry < DateTime.UtcNow)
            return Results.Unauthorized();

        if (refreshToken?.User is null)
            return Results.Unauthorized();

        var newJwt = tokenService.GenerateJwt(refreshToken.User);
        var newRefreshToken = tokenService.GenerateRefreshTokenString();

        refreshToken.Token = newRefreshToken;
        refreshToken.UtcExpiry = DateTime.UtcNow.AddDays(14);

        await context.SaveChangesAsync(cancellationToken);

        httpContext.Response.Cookies.Append("refreshToken", newRefreshToken, TokenService.CreateSecureCookieOptions());

        return Results.Ok(newJwt);
    }
}
