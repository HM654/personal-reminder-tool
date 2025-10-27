using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PersonalReminderTool.Api.Data.Entities;

namespace PersonalReminderTool.Api.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
}
