using PersonalReminderTool.Api.Data.Entities;
using TickerQ.Utilities;
using TickerQ.Utilities.Base;
using TickerQ.Utilities.Interfaces.Managers;
using TickerQ.Utilities.Models;
using TickerQ.Utilities.Models.Ticker;

namespace PersonalReminderTool.Api.Features.Reminders.Services;

internal sealed class ReminderService(ITimeTickerManager<TimeTicker> timeTickerManager, IEmailService emailService, ISmsService smsService)
{
    public async Task CreateReminderAsync(Reminder reminder)
    {
        await timeTickerManager.AddAsync(new TimeTicker
        {
            Request = TickerHelper.CreateTickerRequest<Reminder>(reminder),
            ExecutionTime = reminder.ScheduledDateTimeUtc,
            Function = nameof(SendReminderAsync),
            Description = $"Reminder for '{reminder.UserEmail}' via {reminder.Platforms}.",
            Retries = 3,
            RetryIntervals = [20, 60, 100]
        });
    }

    [TickerFunction(nameof(SendReminderAsync))]
    public async Task SendReminderAsync(TickerFunctionContext<Reminder> tickerContext, CancellationToken cancellationToken) => await HandleReminderAsync(tickerContext.Request, cancellationToken);

    public async Task HandleReminderAsync(Reminder reminder, CancellationToken cancellationToken)
    {
        var platforms = reminder.Platforms;

        if (platforms.HasFlag(Platforms.SMS) && reminder.UserPhoneNumber is not null)
        {
            await smsService.SendSmsAsync(reminder.UserPhoneNumber, reminder.ReminderMessage, cancellationToken);
        }
        if (platforms.HasFlag(Platforms.Email))
        {
            await emailService.SendEmailAsync(reminder.UserEmail, "Reminder", reminder.ReminderMessage, cancellationToken);
        }
    }
}