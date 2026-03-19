using Data;
using Data.Commands.Queue;
using Data.Model;
using Data.Queries;
using Discord.Dto;
using NetCord.Rest;
using Shared.Extensions;
using Shared.Queue;

namespace Discord;

public class CommandOrchestrator
{
    private readonly DiscordService _discordService;
    private readonly QueueDataService _queueDataService;
    private readonly LeagueDataService _leagueDataService;
    public CommandOrchestrator(
        DiscordService discordService,
        QueueDataService queueDataService,
        LeagueDataService leagueDataService)
    {
        _discordService = discordService;
        _leagueDataService = leagueDataService;
        _queueDataService = queueDataService;
    }

    private async Task<Player> GetPlayerByDiscordIdAsync(ulong? discordId, string? discordHandle = null) 
    => await _leagueDataService.RunQueryAsync(
            new GetPlayerQuery()
            {
                DiscordId = discordId,
                DiscordHandle = discordHandle
            });
    
    public async Task<string> GetOpponentAsync(GetOpponentAsyncInDto inDto)
    {
        var resGetP = await GetPlayerByDiscordIdAsync(inDto.PlayerDiscordId, inDto.PlayerDiscordHandle);

        if (resGetP == null)
            return "We don't have your Discord Id yet, please add your Discord user to your Leago profile to enable this command";
        
        var resGetO = await _leagueDataService.RunQueryAsync(new GetOpponentQuery()
            {
                PlayerId = resGetP.Id,
                Round = inDto.Round
            });

        if (resGetO == null)
            return "You are not Participating in this Round";
        
        string response = $"Your opponent for Round {inDto.Round} is {resGetO.FirstName} {resGetO.LastName}" + Environment.NewLine;

        response += "Discord: " + ((resGetO.DiscordId != null) ? _discordService.MentionUser((ulong)resGetO.DiscordId) : "Unavailable") + Environment.NewLine;
        response += "OGS: " + resGetO.OGSHandle + Environment.NewLine;
        response += "Email: " + resGetO.EmailAddress;

        return response;
    }

    public void QueueRankChangeCommand(RankChangePayload rankChangePayloadPayload)
    {
        _queueDataService.ExecuteAsync(new InsertCommandMessageCommand()
        {
            NewCommand = new CommandMessage()
            {
                Type = "RankChangeCommand",
                Payload = rankChangePayloadPayload.SerializePayload()
            }
        });
    }
    public void QueueApplyForAwardCommand(AwardApplicationPayload awardApplicationPayload)
    {
        _queueDataService.ExecuteAsync(new InsertCommandMessageCommand()
        {
            NewCommand = new CommandMessage()
            {
                Type = "AwardApplicationCommand",
                Payload = awardApplicationPayload.SerializePayload()
            }
        });
    }

    public void QueueReportForfeitCommand(ReportForfeitPayload reportForfeitPayload)
    {
        _queueDataService.ExecuteAsync(new InsertCommandMessageCommand()
        {
            NewCommand = new CommandMessage()
            {
                Type = "ReportForfeitCommand",
                Payload = reportForfeitPayload.SerializePayload()
            }
        });
    }
}