using PersonalReminderTool.Api.Features.Reminders.Endpoints;
using PersonalReminderTool.Api.Features.Users.Endpoints;

namespace PersonalReminderTool.Api.Extensions;

internal static class RouteBuilder
{
    public static void MapEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api");

        UserEndpoints.MapEndpoints(api);
        ReminderEndpoints.MapEndpoints(api);
    }
}
