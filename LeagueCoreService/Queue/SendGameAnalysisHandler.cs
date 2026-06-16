using Data;
using Data.Commands.Match;
using Data.Commands.Queue;
using Data.Model;
using Data.Queries;
using Mail.MessageBuilders;
using Shared.Enum;
using Shared.Extensions;
using Shared.Queue;

namespace LeagueCoreService.Queue;

public class SendGameAnalysisHandler(LeagueDataService leagueDataService,
    QueueDataService queueDataService) : ICommandHandler
{
    public string CommandType => "SendGameAnalysis";
    public async Task HandleAsync(CommandMessage cmd)
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