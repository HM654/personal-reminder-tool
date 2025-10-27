namespace PersonalReminderTool.Api.Data.Entities;

[Flags]
internal enum Platforms
{
    None = 0,
    SMS = 1 << 0,
    Email = 1 << 1
}
