using Shared.Dto.Discord;
using Microsoft.Extensions.Hosting;

using NetCord.Hosting.Gateway;
using NetCord.Rest;
using Shared.Settings;
using NetCord;
using Microsoft.Extensions.Options;
using Shared.Dto;
using Shared.Enum;
using System;

namespace Discord
{
    public class DiscordService
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

        protected MessageProperties BuildUpcomingMatchesNotification(MatchDto[] matches)
        {
            string matchList = string.Join(Environment.NewLine + Environment.NewLine, matches.Select(BuildMatchString));

            string content = DiscordTemplates.UpcomingMatchesIntroMessage + Environment.NewLine
                + matchList + Environment.NewLine + Environment.NewLine
                + $"<@&{_settings.Value.MatchAnnouncementRoleId}>" ;

            return new MessageProperties()
                .WithContent(content)
                .WithAllowedMentions(new AllowedMentionsProperties().WithAllowedRoles(null))
                .WithFlags(MessageFlags.SuppressEmbeds);
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

    }
}
