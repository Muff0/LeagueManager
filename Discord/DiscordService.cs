using Discord.MessageBuilders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetCord;
using NetCord.Rest;
using Shared;
using Shared.Dto;
using Shared.Dto.Discord;
using Shared.Enum;
using Shared.Settings;

namespace Discord;

public class DiscordService : ServiceBase
{
    private readonly RestClient _restClient;
    private readonly IOptions<DiscordSettings> _settings;

    public DiscordService(IOptions<DiscordSettings> settings,
        ILogger<DiscordService> logger) : base(logger)
    {
        _settings = settings;
        var to = new BotToken(_settings.Value.Token);
        _restClient = new RestClient(to);
    }

    public async Task<ulong?> GetDiscordUserId(string username)
    {
        await foreach (var user in _restClient.GetGuildUsersAsync(_settings.Value.ServerId))
            if (user.Username.Equals(username) || (user.Nickname?.Equals(username) ?? false))
                return user.Id;

        return null;
    }

    public async Task PostPoll(DiscordPoll poll)
    {
        var message = new MessageProperties()
            .WithPoll(
                new MessagePollProperties(
                        new MessagePollMediaProperties()
                            .WithText(poll.Question),
                        poll.Answers.Select(po =>
                            new MessagePollAnswerProperties(
                                new MessagePollMediaProperties()
                                    .WithText(po.Text)
                                    .WithEmoji(EmojiProperties.Standard(po.Emoji)))))
                    .WithDurationInHours(120));

        await _restClient.SendMessageAsync(_settings.Value.PollChannelId, message);
    }


    public async Task PostReviewThread(PostReviewThreadInDto inDto)
    {
        var match = inDto.Match;
        if (match == null)
            throw new InvalidOperationException("Invalid Match");

        var content = string.Format(DiscordTemplates.ReviewThreadMessage,
            BuildTeacherString(inDto.Teacher),
            match.Players?[0]?.Player?.DiscordId == null
                ? ""
                : MentionUser((ulong)match.Players?[0]?.Player?.DiscordId!),
            match.Players?[1]?.Player?.DiscordId == null
                ? ""
                : MentionUser((ulong)match.Players?[1]?.Player?.DiscordId!),
            inDto.Review.ReviewUrl);

        var title = "GM League - " + inDto.Review.SeasonTitle + " " + match.BuildMatchTitle();

        var msg = BuildThreadProperties(title, content);

        await _restClient.CreateForumGuildThreadAsync(
            _settings.Value.ReviewChannelId,
            msg);
    }

    public async Task RemoveRolesFromUsers(RemoveRoleFromUsersInDto inDto)
    {
        foreach (var userId in inDto.UserIds)
            await _restClient.RemoveGuildUserRoleAsync(_settings.Value.ServerId, userId, inDto.RoleId);
    }

    public async Task SendReviewEventNotification(SendReviewEventNotificationInDto inDto)
    {
        var gamelist = string.Join(Environment.NewLine, inDto.Reviews.Select(mm => mm.BuildMatchTitle()));

        var guildEvent = await CreateReviewEvent(inDto);

        var content = string.Format(DiscordTemplates.ReviewEventNoticeMessage,
            MentionRole(_settings.Value.PlayerRoleId),
            BuildTeacherString(inDto.Teacher),
            gamelist,
            MentionChannel(_settings.Value.ReviewChannelId),
            BuildEventUrl(guildEvent));

        var msg = BuildMessageProperties(content);

        await _restClient.SendMessageAsync(_settings.Value.LeagueAnnouncementsChannelId, msg);
    }

    protected string BuildEventUrl(GuildScheduledEvent guildEvent)
    {
        return $"https://discord.com/events/{guildEvent.GuildId}/{guildEvent.Id}";
    }

    public async Task SendReviewSchedule(ReviewScheduleDto[] reviews)
    {
        var scheduleText = string.Join(Environment.NewLine, reviews.Select(BuildPlayerScheduleString));

        var content = string.Format(DiscordTemplates.ReviewScheduleMessage,
            scheduleText,
            MentionChannel(_settings.Value.ReviewChannelId),
            MentionUser(_settings.Value.AdminId));

        var msg = BuildMessageProperties(content);

        await _restClient.SendMessageAsync(_settings.Value.LeagueAnnouncementsChannelId, msg);
    }

    public async Task SendRoundStartNotification(SendRoundStartNotificationInDto inDto)
    {
        try
        {
            var msg = BuildRoundStartNotification(inDto.RoundNumber, inDto.RoundEnd);

            await _restClient.SendMessageAsync(_settings.Value.LeagueAnnouncementsChannelId, msg);
        }
        catch (Exception ex)
        {
        }
    }

    public async Task SendUpcomingMatchesNotification(SendUpcomingMatchesNotificationInDto inDto)
    {
        try
        {
            var msg = BuildUpcomingMatchesNotification(inDto.Matches);

            await _restClient.SendMessageAsync(_settings.Value.MatchAnnouncementChannelId, msg);
        }
        catch (Exception ex)
        {
        }
    }

    public async Task UpdatePlayerRole(UpdatePlayerRoleInDto inDto)
    {
        var role = await _restClient.GetGuildRoleAsync(_settings.Value.ServerId, _settings.Value.PlayerRoleId);
        var orderedUsers = inDto.CurrentPlayers.Order().ToList();

        await foreach (var user in _restClient.GetGuildUsersAsync(_settings.Value.ServerId))
        {
            var existingIndex = orderedUsers.BinarySearch(user.Id);
            if (user.RoleIds.Contains(role.Id))
            {
                if (existingIndex >= 0)
                    continue;

                await user.RemoveRoleAsync(role.Id);
            }
            else if (existingIndex >= 0)
            {
                await user.AddRoleAsync(role.Id);
            }
        }
    }

    protected string BuildMatchString(MatchDto match)
    {
        var res = "**" + match.BuildMatchTitle() + "**" + Environment.NewLine
                  + "Game Time: " + BuildTimeTag(match.ScheduleTime.GetValueOrDefault()) + Environment.NewLine
                  + match.GameLink;
        return res;
    }

    
    
    protected MessageProperties BuildMessageProperties(string content)
    {
        return new MessageProperties()
            .WithContent(content)
            .WithAllowedMentions(new AllowedMentionsProperties().WithAllowedRoles(null).WithAllowedUsers(null));
    }

    protected string BuildPlayerScheduleString(ReviewScheduleDto reviewSchedule)
    {
        var playerName = reviewSchedule.DiscordId == null
            ? reviewSchedule.OwnerName
            : MentionUser((ulong)reviewSchedule.DiscordId);
        return playerName + " : Rounds " + string.Join(",", reviewSchedule.ReviewedRounds);
    }

    protected MessageProperties BuildRoundStartNotification(int round, DateTime endDate)
    {
        var content =
            string.Format(DiscordTemplates.RoundStartMessage, round, BuildTimeTag(endDate),
                $"<@{_settings.Value.AdminId}>") + Environment.NewLine
                                                 + $"<@&{_settings.Value.LeagueAnnouncementRoleId}>" +
                                                 Environment.NewLine
                                                 + $"<@&{_settings.Value.PlayerRoleId}>";

        return new MessageProperties()
            .WithContent(content)
            .WithAllowedMentions(new AllowedMentionsProperties().WithAllowedRoles(null).WithAllowedUsers(null))
            .WithFlags(MessageFlags.SuppressEmbeds);
    }

    protected string BuildTeacherString(TeacherDto teacher)
    {
        return teacher.Name + " " + MentionUser(teacher.DiscordId) + " " + teacher.Rank.GetDisplayName();
    }

    protected string BuildTimeTag(DateTime time)
    {
        return $"<t:{new DateTimeOffset(time, new TimeSpan()).ToUnixTimeSeconds()}:f>";
    }

    protected MessageProperties BuildUpcomingMatchesNotification(MatchDto[] matches)
    {
        var matchList = string.Join(Environment.NewLine + Environment.NewLine,
            matches.Select(ma => BuildMatchString(ma)));

        var content = DiscordTemplates.UpcomingMatchesIntroMessage + Environment.NewLine
                                                                   + matchList + Environment.NewLine +
                                                                   Environment.NewLine
                                                                   + $"<@&{_settings.Value.MatchAnnouncementRoleId}>";

        return new MessageProperties()
            .WithContent(content)
            .WithAllowedMentions(new AllowedMentionsProperties().WithAllowedRoles(null))
            .WithFlags(MessageFlags.SuppressEmbeds);
    }

    protected async Task<GuildScheduledEvent> CreateReviewEvent(SendReviewEventNotificationInDto inDto)
    {
        var gamelist = string.Join(Environment.NewLine, inDto.Reviews.Select(mm => mm.BuildMatchTitle()));

        var content = string.Format(DiscordTemplates.ReviewEventBodyMessage,
            BuildTeacherString(inDto.Teacher),
            gamelist,
            MentionChannel(_settings.Value.ReviewChannelId));

        var eventProperties = new GuildScheduledEventProperties("Review Night",
                GuildScheduledEventPrivacyLevel.GuildOnly,
                new DateTimeOffset(inDto.DateTimeUTC, TimeSpan.Zero),
                GuildScheduledEventEntityType.StageInstance)
            .WithChannelId(_settings.Value.LiveReviewChannelId)
            .WithDescription(content);

        var resEvent = await _restClient.CreateGuildScheduledEventAsync(_settings.Value.ServerId, eventProperties);

        return resEvent;
    }

    public string MentionChannel(ulong channelId)
    {
        return $"<#{channelId}>";
    }

    public string MentionRole(ulong roleId)
    {
        return $"<@&{roleId}>";
    }

    public string MentionUser(ulong userId)
    {
        return $"<@{userId}>";
    }

    private ForumGuildThreadProperties BuildThreadProperties(string title, string content)
    {
        return new ForumGuildThreadProperties(
            title,
            new ForumGuildThreadMessageProperties()
                .WithAllowedMentions(null)
                .WithContent(content));
    }

    public async Task SendRoundStatsMessage(StreakDataDto streakData)
    {
        var messageProperties = BuildMessageProperties(StreakMessageBuilder.Compose(streakData));

        await _restClient.SendMessageAsync(_settings.Value.AdminNotificationChannelId, messageProperties);
    }

    public async Task SendPollNotification(int pollsInQueue, DateTime pollTime, string message,
        DiscordNotificationType notificationType)
    {
        var content = message + Environment.NewLine
                              + $"There are now {pollsInQueue} polls left in the queue." + Environment.NewLine
                              + "Next poll will be posted on " + BuildTimeTag(pollTime) + Environment.NewLine;

        if (notificationType == DiscordNotificationType.Admin)
            content += MentionUser(_settings.Value.AdminId);
        else if (notificationType == DiscordNotificationType.Poll)
            content += MentionUser(_settings.Value.PollCuratorId);

        var messageProperties = BuildMessageProperties(content);

        await _restClient.SendMessageAsync(_settings.Value.AdminNotificationChannelId, messageProperties);
    }
}