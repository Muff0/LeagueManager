namespace Shared.Queue;

public class SendEmailPayload
{
    public string HtmlBody { get; set; }
    public string ToAddress { get; set; }
    public string ToName { get; set; }
    public string[] Ccs { get; set; }
    public string[] Bccs { get; set; }
    public string Subject { get; set; }
}