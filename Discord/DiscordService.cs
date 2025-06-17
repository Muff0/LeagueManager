using Microsoft.Extensions.Options;
using NetCord;
using NetCord.Rest;
using Shared;
using Shared.Dto;
using Shared.Dto.Discord;
using Shared.Enum;
using Shared.Settings;

namespace Discord
{
    public class DiscordService : ServiceBase
    {
        private readonly RestClient _client;
        private readonly IOptions<DiscordSettings> _settings;

        public DiscordService(IOptions<DiscordSettings> settings)
        {
            _settings = settings;
            var to = new BotToken(_settings.Value.Token);
            _client = new RestClient(to);
        }

        protected string BuildMatchTitle(MatchDto match)
        {
            var whitePlayer = match.Players?.FirstOrDefault(pm => pm.Color == Shared.Enum.PlayerColor.White);
            var blackPlayer = match.Players?.FirstOrDefault(pm => pm.Color == Shared.Enum.PlayerColor.Black);
            if (whitePlayer?.Player == null || blackPlayer?.Player == null)
                return "";

            return string.Format(DiscordTemplates.MatchTitleTemplate,
                match.Round,
                blackPlayer.Player.FirstName + " " + blackPlayer.Player.LastName,
                blackPlayer.Player.Rank.GetDisplayName(),
                whitePlayer.Player.FirstName + " " + whitePlayer.Player.LastName,
                whitePlayer.Player.Rank.GetDisplayName());
        }

        protected string BuildTimeTag(DateTime time)
        {
            return $"<t:{new DateTimeOffset(time).ToUnixTimeSeconds()}:f>";
        }

        protected string BuildMatchString(MatchDto match)
        {
            string res = "**" + BuildMatchTitle(match) + "**" + Environment.NewLine
                + "Game Time: " + BuildTimeTag(match.ScheduleTime.GetValueOrDefault()) + Environment.NewLine
                + match.GameLink;
            return res;
        }

        protected MessageProperties BuildRoundStartNotification(int round, DateTime endDate)
        {
            string content = string.Format(DiscordTemplates.RoundStartMessage, round, BuildTimeTag(endDate), $"<@{_settings.Value.AdminId}>") + Environment.NewLine
                + $"<@&{_settings.Value.LeagueAnnouncementRoleId}>" + Environment.NewLine
                + $"<@&{_settings.Value.PlayerRoleId}>";

            return new MessageProperties()
                .WithContent(content)
                .WithAllowedMentions(new AllowedMentionsProperties().WithAllowedRoles(null).WithAllowedUsers(null))
                .WithFlags(MessageFlags.SuppressEmbeds);
        }

        protected MessageProperties BuildUpcomingMatchesNotification(MatchDto[] matches)
        {
            string matchList = string.Join(Environment.NewLine + Environment.NewLine, matches.Select(BuildMatchString));

            string content = DiscordTemplates.UpcomingMatchesIntroMessage + Environment.NewLine
                + matchList + Environment.NewLine + Environment.NewLine
                + $"<@&{_settings.Value.MatchAnnouncementRoleId}>";

            return new MessageProperties()
                .WithContent(content)
                .WithAllowedMentions(new AllowedMentionsProperties().WithAllowedRoles(null))
                .WithFlags(MessageFlags.SuppressEmbeds);
        }

        public async Task UpdatePlayerRole(UpdatePlayerRoleInDto inDto)
        {
            var role = await _client.GetGuildRoleAsync(_settings.Value.ServerId, _settings.Value.PlayerRoleId);
            var orderedUsers = inDto.CurrentPlayers.Order().ToList();

            await foreach (var user in _client.GetGuildUsersAsync(_settings.Value.ServerId))
            {
                var existingIndex = orderedUsers.BinarySearch(user.Id);
                if (user.RoleIds.Contains(role.Id))
                {
                    if (existingIndex >= 0)
                        continue;

                    await user.RemoveRoleAsync(role.Id);
                }
                else if (existingIndex >= 0)
                    await user.AddRoleAsync(role.Id);
            }
        }

        public async Task<ulong?> GetDiscordUserId(string username)
        {
            await foreach (var user in _client.GetGuildUsersAsync(_settings.Value.ServerId))
            {
                if (user.Username.Equals(username) || (user.Nickname?.Equals(username) ?? false))
                {
                    return user.Id;
                }
            }
            return null;
        }

        public async Task RemoveRolesFromUsers(RemoveRoleFromUsersInDto inDto)
        {
            foreach (var userId in inDto.UserIds)
            {
                await _client.RemoveGuildUserRoleAsync(_settings.Value.ServerId, userId, inDto.RoleId);
            }
        }

        public async Task<ulong[]> FindUsersWithRole(ulong roleId)
        {
            var res = new List<ulong>();
            await foreach (var user in _client.GetGuildUsersAsync(_settings.Value.ServerId))
            {
                if (user.RoleIds.Contains(roleId))
                    res.Add(user.Id);
            }

            return res.ToArray();
        }

        public async Task AssignRoleToUsers(AssignRoleToUsersInDto inDto)
        {
            try
            {
                var users = _client.SearchGuildUsersAsync(_settings.Value.ServerId).ToBlockingEnumerable();

                foreach (string userId in inDto.Users)
                {
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

        public async Task SendRoundStartNotification(SendRoundStartNotificationInDto inDto)
        {
            try
            {
                var msg = BuildRoundStartNotification(inDto.RoundNumber, inDto.RoundEnd);

                await _client.SendMessageAsync(_settings.Value.MatchAnnouncementChannelId, msg);
            }
            catch (Exception ex)
            {
                return;
            }
        }

        public async Task SendUpcomingMatchesNotification(SendUpcomingMatchesNotificationInDto inDto)
        {
            try
            {
                var msg = BuildUpcomingMatchesNotification(inDto.Matches);

                await _client.SendMessageAsync(_settings.Value.MatchAnnouncementChannelId, msg);
            }
            catch (Exception ex)
            {
                return;
            }
        }

        protected string MentionChannel(ulong channelId) => $"<@&{channelId}>";
        protected string MentionUser(ulong userId) => $"<@{userId}>";

        protected MessageProperties BuildMessageProperties(string content) => new MessageProperties()
                .WithContent(content)
                .WithAllowedMentions(new AllowedMentionsProperties().WithAllowedRoles(null).WithAllowedUsers(null));


        protected string BuildPlayerScheduleString(ReviewScheduleDto reviewSchedule)
        {
            string playerName = reviewSchedule.DiscordId == null ? reviewSchedule.OwnerName : MentionUser((ulong)reviewSchedule.DiscordId);
            return playerName + " : Rounds " + string.Join(",", reviewSchedule.ReviewedRounds);
        }

        public async Task SendReviewSchedule(ReviewScheduleDto[] reviews)
        {
            var scheduleText = string.Join(Environment.NewLine, reviews.Select(BuildPlayerScheduleString));

            var content = string.Format(DiscordTemplates.ReviewScheduleMessage, 
                scheduleText,
                MentionChannel(_settings.Value.ReviewChannelId), 
                MentionUser(_settings.Value.AdminId));

            var msg = BuildMessageProperties(content);

            await _client.SendMessageAsync(_settings.Value.MatchAnnouncementChannelId, msg);
        }
    }
}