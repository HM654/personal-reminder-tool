namespace PersonalReminderTool.Api.Features.Reminders.Services;

internal interface IEmailService
{
    public Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken);
}
