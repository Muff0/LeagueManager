namespace Shared.Settings
{
    public class DiscordSettings
    {
        public ulong AppId { get; init; } 
        public string PublicKey { get; init; } = string.Empty;
        public string Token { get; init; } = string.Empty;
        public ulong ReviewChannelId { get; init; }
        public ulong MatchAnnouncementChannelId { get; init; }
        public ulong LeagueAnnouncementsChannelId { get; init; }
        public ulong MatchAnnouncementRoleId { get; init; }
        public ulong LeagueAnnouncementRoleId { get; init; }
        public ulong ServerId { get; init; }
        public ulong AdminId { get; init; }
        public ulong PlayerRoleId { get; init; } = 0;
        public ulong LiveReviewChannelId { get; init; }
        public ulong PollChannelId { get; init; }
        public ulong AdminNotificationChannelId { get; init; }
        public ulong PollCuratorId { get; init; }
        public string? AlertWebhookUrl { get; set; } 
    }
}