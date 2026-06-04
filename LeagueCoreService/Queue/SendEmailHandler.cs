using Data;
using Data.Model;
using Mail;
using Shared.Queue;

namespace LeagueCoreService.Queue;

public class SendEmailHandler(MailService mailService) 
    : ICommandHandler
{
    public string CommandType => "SendEmail";
    public async Task HandleAsync(CommandMessage cmd)
    {
        var payload = cmd.GetPayload<SendEmailPayload>();

        await mailService.SendAsync(payload);
    }
}