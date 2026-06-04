using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Shared.Queue;
using Shared.Settings;

namespace Mail;

public class MailService(IOptions<MailSettings> options, ILogger<MailService> logger)
{
    private readonly MailSettings _settings = options.Value;

    private readonly string Signature =
        """<br clear="all"></div><div><div dir="ltr" class="gmail_signature" data-smartmail="gmail_signature"><div dir="ltr"><span><div><p class="MsoNormal"><b>Pietro <br></b></p><p class="MsoNormal"><b>Tournament Director<br></b></p><p class="MsoNormal"><b><a href="https://gomagic.org/" target="_blank" data-saferedirecturl="https://www.google.com/url?q=https://gomagic.org/&amp;source=gmail&amp;ust=1780259372360000&amp;usg=AOvVaw31FH_rgpiQZMchmCjdgH6I"><img src="https://ci3.googleusercontent.com/meips/ADKq_NZhW3TeGXQ0kahXOkaTBTV2Eu5YPymLi8UN_BDvqUgIJLLNGSFrBgLAXUr325ZYRC07c_hlGvg6Xr08vWMmO5lg6xCFXuc05SiL2MKUnv-h1lQxEws1qIPSjzFT-cEclOGbdnWVjZotlN2M0-FSOQcRTLM6BF0gbQjDrXQtqtd1s5fxlMDcWKg4CwCbYFuC_S3nuGVJpgptG5JsoNnjzzV-uAeSscno8Uc4vVNB3q1DSxsmwKILQvGc3VA3EeB_F0wAaBWWfr1Dy4wbr2cDvIioRZbmAt0pDubTRwUF=s0-d-e1-ft#https://img.posteml.com/en/v5/user-files?userId=6193633&amp;resource=himg&amp;disposition=inline&amp;name=6iymtxstz9gatumke5xi5nsmwb3dtw65xdega4ukxgq56x6kthsa5g7mw6rrazb7f7jeex9s83uy45gtn69376bud8fwtwop8mmuu1ze" width="200" height="46" class="CToWUd" data-bit="iit">""";

    /// <summary>
    /// Sends a single email. HTML body is required; a plain-text fallback is
    /// derived automatically by stripping tags.
    /// </summary>
    public async Task SendAsync(
        string toAddress,
        string toName,
        string subject,
        string htmlBody,
        CancellationToken ct = default)
    {
        var message = BuildMessage([(toAddress, toName)], subject, htmlBody);
        await SendMessageAsync(message, ct);
    }
    /// <summary>
    /// Sends a single email. HTML body is required; a plain-text fallback is
    /// derived automatically by stripping tags.
    /// Overload for Queue Payload
    /// </summary>
    public async Task SendAsync(
        SendEmailPayload payload,
        CancellationToken ct = default)
        {
            var message = BuildMessage([(payload.ToAddress, payload.ToName)], 
                payload.Subject, 
                payload.HtmlBody,
                payload.Ccs.Select(cc =>  (cc, cc)).ToArray(),
                payload.Bccs.Select(bcc =>  (bcc, bcc)).ToArray());
        await SendMessageAsync(message, ct);
    }

    

    /// <summary>
    /// Sends the same email to multiple recipients as individual messages.
    /// Reuses a single SMTP connection for the whole batch.
    /// </summary>
    public async Task SendBulkAsync(
        IEnumerable<(string Address, string Name)> recipients,
        string subject,
        string htmlBody,
        CancellationToken ct = default)
    {
        var recipientList = recipients.ToList();
        if (recipientList.Count == 0) return;

        using var smtp = await ConnectAsync(ct);

        foreach (var (address, name) in recipientList)
        {
            ct.ThrowIfCancellationRequested();
            try
            {
                var message = BuildMessage([(address, name)], subject, htmlBody);
                await smtp.SendAsync(message, ct);
                logger.LogDebug("Mail sent to {Address}", address);
            }
            catch (Exception ex)
            {
                // Log and continue — one bad address shouldn't abort the batch
                logger.LogError(ex, "Failed to send mail to {Address}", address);
            }
        }

        await smtp.DisconnectAsync(true, ct);
    }

    // -------------------------------------------------------------------------

    private async Task SendMessageAsync(MimeMessage message, CancellationToken ct)
    {
        using var smtp = await ConnectAsync(ct);
        await smtp.SendAsync(message, ct);
        await smtp.DisconnectAsync(true, ct);
    }

    private async Task<SmtpClient> ConnectAsync(CancellationToken ct)
    {
        var smtp = new SmtpClient();
        await smtp.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, SecureSocketOptions.Auto, ct);
        await smtp.AuthenticateAsync(_settings.Username, _settings.Password, ct);
        return smtp;
    }

    private MimeMessage BuildMessage(
        IEnumerable<(string Address, string Name)> recipients,
        string subject,
        string htmlBody,
        IEnumerable<(string Address, string Name)>? ccs = null,
        IEnumerable<(string Address, string Name)>? bccs = null)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromAddress));

        foreach (var (address, name) in recipients)
            message.To.Add(new MailboxAddress(name, address));

        if (ccs != null)
            foreach (var (address, name)  in ccs)
                message.Cc.Add(new MailboxAddress(name, address));
            
        if (bccs != null)
            foreach (var (address, name)  in bccs)
                message.Bcc.Add(new MailboxAddress(name, address));
        
        message.Subject = subject;

        var builder = new BodyBuilder
        {
            HtmlBody = htmlBody + Signature,
            TextBody = HtmlToPlainText(htmlBody + Signature)
        };
        message.Body = builder.ToMessageBody();

        return message;
    }

    private static string HtmlToPlainText(string html) =>
        System.Text.RegularExpressions.Regex
            .Replace(html, "<[^>]+>", string.Empty)
            .Trim();
}