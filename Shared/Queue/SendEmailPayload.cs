namespace Shared.Queue;

public class SendEmailPayload : ICommandPayload
{
    public string HtmlBody { get; set; } = string.Empty;
    public string ToAddress { get; set; }= string.Empty;
    public string ToName { get; set; }= string.Empty;
    public string[] Ccs { get; set; } = [];
    public string[] Bccs { get; set; } = [];
    public string Subject { get; set; }= string.Empty;
}