using Data;
using Discord;
using LeagoService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared;
using Shared.Settings;

namespace LeagueCoreService.Services;

public class MainService : ServiceBase
{
    private readonly LeagoMainService _leagoService;

    private readonly LeagueDataService _leagueDataService;

    public MainService(LeagoMainService leagoService,
        IOptions<LeagoSettings> leagoOptions,
        IDbContextFactory<LeagueContext> leagueContextFactory,
        LeagueDataService dataService,
        DiscordService discordService,
        QueueDataService queueService,
        ILogger<MainService> logger) : base(logger)
    {
        _leagoService = leagoService;
        _leagueDataService = dataService;
    }
}