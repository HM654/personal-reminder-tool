using NSubstitute;
using PersonalReminderTool.Api.Data.Entities;
using PersonalReminderTool.Api.Features.Reminders.Services;
using TickerQ.Utilities.Interfaces.Managers;
using TickerQ.Utilities.Models.Ticker;

namespace PersonalReminderTool.Tests;

public sealed class ReminderServiceTests
{
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;
    private readonly ReminderService _reminderService;

    public ReminderServiceTests()
    {
        var tickerTimeManagerSubstitute = Substitute.For<ITimeTickerManager<TimeTicker>>();
        _emailService = Substitute.For<IEmailService>();
        _smsService = Substitute.For<ISmsService>();
        _reminderService = new ReminderService(tickerTimeManagerSubstitute, _emailService, _smsService);
    }

    [Fact]
    public async Task HandleReminderAsync_WithBothPlatforms_SendsBoth()
    {
        // Arrange
        var reminder = new Reminder
        {
            UserEmail = "test@example.com",
            UserPhoneNumber = "+1234567890",
            Platforms = Platforms.Email | Platforms.SMS,
            ReminderMessage = "Test reminder",
            ScheduledDateTimeUtc = DateTime.UtcNow,
        };

        // Act
        await _reminderService.HandleReminderAsync(reminder, CancellationToken.None);

        // Assert
        await _emailService.Received(0)
            .SendEmailAsync(reminder.UserEmail, "Reminder", reminder.ReminderMessage, CancellationToken.None);
        await _smsService.Received(1)
            .SendSmsAsync(reminder.UserPhoneNumber, reminder.ReminderMessage, CancellationToken.None);
    }

    [Fact]
    public async Task HandleReminderAsync_WithEmailOnly_SendsEmailOnly()
    {
        // Arrange
        var reminder = new Reminder
        {
            UserEmail = "test@example.com",
            UserPhoneNumber = "+1234567890",
            Platforms = Platforms.Email,
            ReminderMessage = "Test reminder",
            ScheduledDateTimeUtc = DateTime.UtcNow,
        };

        // Act
        await _reminderService.HandleReminderAsync(reminder, CancellationToken.None);

        // Assert
        await _emailService.Received(1)
            .SendEmailAsync(reminder.UserEmail, "Reminder", reminder.ReminderMessage, CancellationToken.None);
        await _smsService.DidNotReceive()
            .SendSmsAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleReminderAsync_WithSmsOnly_SendsSmsOnly()
    {
        // Arrange
        var reminder = new Reminder
        {
            UserEmail = "test@example.com",
            UserPhoneNumber = "+1234567890",
            Platforms = Platforms.SMS,
            ReminderMessage = "Test reminder",
            ScheduledDateTimeUtc = DateTime.UtcNow,
        };

        // Act
        await _reminderService.HandleReminderAsync(reminder, CancellationToken.None);

        // Assert
        await _emailService.DidNotReceive()
            .SendEmailAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _smsService.Received(1)
            .SendSmsAsync(reminder.UserPhoneNumber, reminder.ReminderMessage, CancellationToken.None);
    }

    [Fact]
    public async Task HandleReminderAsync_WithSmsOnlyButNullPhoneNumber_SendsNothing()
    {
        // Arrange
        var reminder = new Reminder
        {
            UserEmail = "test@example.com",
            UserPhoneNumber = null,
            Platforms = Platforms.SMS,
            ReminderMessage = "Test reminder",
            ScheduledDateTimeUtc = DateTime.UtcNow,
        };

        // Act
        await _reminderService.HandleReminderAsync(reminder, CancellationToken.None);

        // Assert
        await _smsService.DidNotReceive()
            .SendSmsAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _emailService.DidNotReceive()
            .SendEmailAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }
}
