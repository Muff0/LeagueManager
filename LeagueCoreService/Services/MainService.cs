using Data;
using Data.Commands.Match;
using Data.Commands.Queue;
using Data.Model;
using Data.Queries;
using Discord;
using LeagoService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared;
using Shared.Dto;
using Shared.Dto.Discord;
using Shared.Enum;
using Shared.Queue;
using Shared.Services;
using Shared.Settings;

namespace LeagueCoreService.Services
{
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
}