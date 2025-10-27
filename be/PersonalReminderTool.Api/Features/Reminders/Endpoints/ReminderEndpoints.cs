using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalReminderTool.Api.Data;
using PersonalReminderTool.Api.Data.Entities;
using PersonalReminderTool.Api.Features.Reminders.Dtos;
using PersonalReminderTool.Api.Features.Reminders.Services;
using System.Security.Claims;

namespace PersonalReminderTool.Api.Features.Reminders.Endpoints;

internal static class ReminderEndpoints
{
    public static void MapEndpoints(this RouteGroupBuilder api)
    {
        api.MapPost("/set-reminder", SetReminderAsync)
        .RequireAuthorization();
    }

    private static async Task<IResult> SetReminderAsync([FromBody] ReminderRequestDto request, HttpContext httpContext, ApplicationDbContext context, ReminderService reminderService, CancellationToken cancellationToken)
    {
        if (request.Platforms == Platforms.None)
            return Results.BadRequest("A valid platform must be selected.");

        if (string.IsNullOrEmpty(request.ReminderMessage))
            return Results.BadRequest("A reminder must contain a message.");

        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
            return Results.Unauthorized();

        var scheduledDateTimeUtc = GetScheduledDateTimeUtc(request.ScheduledDateTime, request.TimeZone);

        if (scheduledDateTimeUtc < DateTime.UtcNow)
            return Results.BadRequest("Cannot schedule a reminder in the past.");

        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
            return Results.NotFound("User not found.");

        await reminderService.CreateReminderAsync(new Reminder
        {
            UserEmail = user.Email,
            UserPhoneNumber = user.PhoneNumber,
            Platforms = request.Platforms,
            ReminderMessage = request.ReminderMessage,
            ScheduledDateTimeUtc = scheduledDateTimeUtc,
        });

        return Results.Ok();
    }

    private static DateTime GetScheduledDateTimeUtc(DateTime scheduledDateTimeLocal, string timeZone)
    {
        var dateTimeUnspecified = DateTime.SpecifyKind(scheduledDateTimeLocal, DateTimeKind.Unspecified);

        try
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            return TimeZoneInfo.ConvertTimeToUtc(dateTimeUnspecified, timeZoneInfo);
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.ConvertTimeToUtc(dateTimeUnspecified, TimeZoneInfo.Local);
        }
    }
}
