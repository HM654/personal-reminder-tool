namespace PersonalReminderTool.Api.Features.Reminders.Services;

internal interface ISmsService
{
    public Task SendSmsAsync(string to, string message, CancellationToken cancellationToken);
}
