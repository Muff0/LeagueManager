namespace Shared.Settings
{
    public class DiscordSettings
    {
        public ulong AppId { get; set; } 
        public string PublicKey { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public ulong ReviewChannelId { get; set; }
        public ulong MatchAnnouncementChannelId { get; set; }
        public ulong MatchAnnouncementRoleId { get; set; }
        public ulong LeagueAnnouncementRoleId { get; set; }
        public ulong ServerId { get; set; }
        public ulong AdminId { get; set; }
        public ulong PlayerRoleId { get; set; } = 0;
        public ulong LiveReviewChannelId { get; set; }
    }
}