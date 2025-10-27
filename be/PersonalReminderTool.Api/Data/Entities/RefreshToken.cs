namespace PersonalReminderTool.Api.Data.Entities;

public class RefreshToken
{
    public required string Id { get; set; }
    public required string Token { get; set; }
    public required DateTime UtcExpiry { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
}
