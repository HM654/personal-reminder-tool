using PersonalReminderTool.Api.Data;
using PersonalReminderTool.Api.Extensions;
using PersonalReminderTool.Api.Features.Reminders.Services;
using PersonalReminderTool.Api.Features.Users.Services;
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

await builder.Services.SetupPostgreSQLDatabaseAsync(builder.Configuration);

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
builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await app.MigrateDatabaseAsync();
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
