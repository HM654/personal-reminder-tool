namespace PersonalReminderTool.Api.Features.Reminders.Services;

public interface ISmsService
{
    Task SendSmsAsync(string to, string message);
}
