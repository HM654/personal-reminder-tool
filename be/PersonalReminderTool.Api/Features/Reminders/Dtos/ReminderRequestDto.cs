using PersonalReminderTool.Api.Data.Entities;

namespace PersonalReminderTool.Api.Features.Reminders.Dtos;

internal sealed record ReminderRequestDto(Platforms Platforms, string ReminderMessage, DateTime ScheduledDateTime, string TimeZone);
