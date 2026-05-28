using Data;
using Data.Commands.Match;
using Data.Model;
using Discord;
using LeagueCoreService.Services;
using Microsoft.Extensions.Options;
using Shared.Dto.Discord;
using Shared.Settings;

namespace LeagueCoreService.Queue;

public class SendUpcomingMatchesNotificationHandler(LeagueDataService leagueDataService,
    ILogger<SendUpcomingMatchesNotificationHandler> logger,
    DiscordService discordService,
    IOptions<SchedulerSettings> settings) 
    : ICommandHandler
{
    public string CommandType => "SendUpcomingMatchesNotification";
    
    public async Task<Data.Model.Match[]> GetUpcomingMatches()
    {
        var query = new Data.Queries.GetMatchesByTimeQuery()
        {
            InlcudePlayers = true,
            IncludeCompleted = false,
            IncludeNotConfirmed = false,
            IsNotificationSent = false,
            TimeFromUTC = DateTime.Now.ToUniversalTime(),
            TimeToUTC = DateTime.Now.AddMinutes(settings.Value.UpcomingMatchesTimeSpanMinutes).ToUniversalTime(),
            Count = 5
        };

        var res = await leagueDataService.RunQueryAsync(query);

        return res.ToArray();
    }

    public async Task SendUpcomingMatchesNotification()
    {
        try
        {
            var resm = await GetUpcomingMatches();

            if (resm.Length == 0)
                return;
            var dto = resm.Select(mm => mm.ToMatchDto()).ToArray();
            await discordService.SendUpcomingMatchesNotification(new SendUpcomingMatchesNotificationInDto()
            {
                Matches = dto
            });
            await leagueDataService.ExecuteAsync(new SetMatchesNotifiedCommand()
            {
                Matches = dto
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex,ex.Message);
        }
    }
    public async Task HandleAsync(CommandMessage cmd)
        => await SendUpcomingMatchesNotification();
}