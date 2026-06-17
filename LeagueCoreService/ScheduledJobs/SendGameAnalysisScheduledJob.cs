using Data;
using Data.Commands.Match;
using Data.Commands.Queue;
using Data.Model;
using Data.Queries;
using Mail.MessageBuilders;
using Newtonsoft.Json;
using Shared.Enum;
using Shared.Extensions;
using Shared.Queue;
using Shared.Services;
using Shared.Settings;

namespace LeagueCoreService.ScheduledJobs;

public class SendGameAnalysisScheduledJob(QueueDataService queueDataService, 
    LeagueDataService leagueDataService,
    TimeIntervalSchedulerService schedulerService)
    : ScheduledJobBase<TimeIntervalSchedulerService>(schedulerService)
{
    public override string JobType => "SendGameAnalysis";

    public override string? DefaultSettingsJson => JsonConvert.SerializeObject(
        new TimeIntervalJobSettings()
        {
            IntervalSeconds = 600
        });
    
    public override async Task ExecuteAsync(string? settingsJson, CancellationToken ct)
    {
        var games = await leagueDataService.RunQueryAsync(
            new GetMatchesQuery()
            {
                IncludePlayers = true,
                WithGameAnalysisStatus = GameAnalysisStatus.Completed
            });

        foreach (var game in games)
        {
            await leagueDataService.ExecuteAsync(new UpdateMatchGameAnalysisCommand()
            {
                MatchId = game.Id,
                SetNewStatus = GameAnalysisStatus.Sent
            });
            
            var message = new GameAnalysisReadyMessage(game.ToMatchDto());
            await queueDataService.ExecuteAsync(new
                InsertCommandMessageCommand()
                {
                    NewCommand = new CommandMessage()
                    {
                        Type = "SendEmail",
                        Payload = new SendEmailPayload()
                        {
                            Subject = message.Subject,
                            HtmlBody = message.HtmlBody,
                            Tos = game.PlayerMatches.Select(pm => pm.Player.EmailAddress).ToArray()
                        }.SerializePayload()
                    }
                });
        }
    }
}