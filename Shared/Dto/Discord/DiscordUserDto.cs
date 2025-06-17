namespace Shared.Dto.Discord
{
    public class DiscordUserDto
    {
        public ulong? Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Nickname { get; set; } = string.Empty;
    }
}