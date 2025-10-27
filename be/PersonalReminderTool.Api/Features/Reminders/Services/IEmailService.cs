﻿namespace PersonalReminderTool.Api.Features.Reminders.Services;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken);
}
