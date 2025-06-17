namespace Shared.Settings
{
    public class DiscordSettings
    {
        public string AppId { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public ulong TestChannelId { get; set; }
        public ulong MatchAnnouncementChannelId { get; set; }
        public ulong MatchAnnouncementRoleId { get; set; }
        public ulong LeagueAnnouncementRoleId { get; set; }
        public ulong ServerId { get; set; }
        public ulong AdminId { get; set; }
        public ulong PlayerRoleId { get; set; } = 0;
    }
}