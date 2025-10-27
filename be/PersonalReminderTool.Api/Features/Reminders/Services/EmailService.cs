using Azure;
using Azure.Communication.Email;

namespace PersonalReminderTool.Api.Features.Reminders.Services;

internal sealed class EmailService(IConfiguration configuration) : IEmailService
{
    private readonly EmailClient _client = new EmailClient(configuration.GetConnectionString("AzureCommunicationServiceConnectionString"));

    public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken)
    {
        var senderAddress = configuration.GetSection("Api").GetValue<string>("EmailSenderAddress")!;

        var emailMessage = new EmailMessage(
            senderAddress: senderAddress,
            content: new EmailContent(subject) { PlainText = body },
            recipients: new EmailRecipients([new EmailAddress(to)]));

        var emailSendOperation = await _client.SendAsync(WaitUntil.Completed, emailMessage, cancellationToken);
    }
}
