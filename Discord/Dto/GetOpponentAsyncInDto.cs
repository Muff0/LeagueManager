namespace Discord.Dto;

public class GetOpponentAsyncInDto
{
    public ulong PlayerDiscordId { get; set; }
    public int Round { get; set; }
    public string PlayerDiscordHandle { get; set; } = string.Empty;
}