namespace PersonalReminderTool.Api.Data.Entities;

internal sealed class User
{
    public required string Id { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public required string Password { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}
