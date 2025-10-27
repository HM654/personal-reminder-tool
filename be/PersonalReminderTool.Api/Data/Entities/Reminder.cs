namespace PersonalReminderTool.Api.Data.Entities;

internal sealed class Reminder
{
    public required string UserEmail { get; set; }
    public string? UserPhoneNumber { get; set; }
    public required Platforms Platforms { get; set; }
    public required string ReminderMessage { get; set; }
    public required DateTime ScheduledDateTimeUtc { get; set; }
}
