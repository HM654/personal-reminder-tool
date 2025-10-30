using Azure.Communication.Sms;

namespace PersonalReminderTool.Api.Features.Reminders.Services;

internal sealed class SmsService(IConfiguration configuration) : ISmsService
{
    private readonly SmsClient _client = new SmsClient(configuration.GetConnectionString("AzureCommunicationServiceConnectionString"));

    public async Task SendSmsAsync(string to, string message, CancellationToken cancellationToken)
    {
        var senderNumber = configuration.GetSection("Api").GetValue<string>("MobileSenderNumber");
        var partnerApiKey = configuration.GetSection("Api").GetValue<string>("PartnerApiKey")!;
        var partner = configuration.GetSection("Api").GetValue<string>("Partner")!;

        await _client.SendAsync(
            from: senderNumber,
            to: to,
            message: message,
            options: new SmsSendOptions(enableDeliveryReport: true)
            {
                Tag = "reminder",
                MessagingConnect = new MessagingConnectOptions(partnerApiKey, partner)
            },
            cancellationToken
        );
    }
}
