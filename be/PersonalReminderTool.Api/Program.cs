using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PersonalReminderTool.Api;
using PersonalReminderTool.Api.Data;
using PersonalReminderTool.Api.Data.Entities;
using PersonalReminderTool.Api.Features.Reminders.Services;
using PersonalReminderTool.Api.Features.Users.Services;
using System.Text;
using TickerQ.Dashboard.DependencyInjection;
using TickerQ.DependencyInjection;
using TickerQ.EntityFrameworkCore.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
string corsPolicy = builder.Configuration["Api:CorsPolicy"]!;

builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicy,
        policy =>
        {
            policy
                .WithOrigins(builder.Configuration["Api:ClientUrl"]!)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));

    // Synchronous seeding to support EF migrations in deployments
    options.UseSeeding((context, _) =>
    {
        var baseUser = builder.Configuration
            .GetSection("BaseUser")
            .Get<User>()!;

        bool exists = context.Set<User>().Any(u => u.Id == baseUser.Id);

        if (!exists)
        {
            context.Set<User>().Add(baseUser);
            context.SaveChanges();
        }
    });
});

builder.Services.AddTickerQ(options =>
{
    options.SetInstanceIdentifier("TickerQ");

    options.AddOperationalStore<ApplicationDbContext>(options =>
    {
        options.UseModelCustomizerForMigrations();
    });

    options.AddDashboard(dboptions =>
    {
        dboptions.EnableBasicAuth = true;
    });
});

builder.Services.AddSingleton<TokenService>();
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton<ISmsService, SmsService>();
builder.Services.AddScoped<ReminderService>();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(jwtOptions =>
    {

        jwtOptions.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
        };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
}

app.UseCors(corsPolicy);
app.UseAuthentication();
app.UseAuthorization();
app.UseTickerQ();

app.MapEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.Run();
