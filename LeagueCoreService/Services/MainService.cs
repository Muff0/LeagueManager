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

        private readonly IOptions<LeagoSettings> _leagoOptions;
        private readonly LeagueDataService _leagueDataService;
        private readonly QueueDataService _queueDataService;
        private readonly DiscordService _discordService;
        private IDbContextFactory<LeagueContext> _leagueContextFactory;

        public MainService(LeagoMainService leagoService,
            IOptions<LeagoSettings> leagoOptions,
            IDbContextFactory<LeagueContext> leagueContextFactory,
            LeagueDataService dataService,
            DiscordService discordService,
            QueueDataService queueService,
            ILogger<MainService> logger) : base(logger)
        {
            _leagueContextFactory = leagueContextFactory;
            _leagoService = leagoService;
            _leagoOptions = leagoOptions;
            _leagueDataService = dataService;
            _queueDataService = queueService;
            _discordService = discordService;
        }



        public async Task UpdateActiveSeason()
        {
            var res2 = await _leagoService.GetLeague(new Shared.Dto.GetLeagueInDto()
            {
                ArenaKey = _leagoOptions.Value.ArenaKey,
                LeagueKey = _leagoOptions.Value.LeagueKey
            });

            if (res2?.Result == null)
                throw new InvalidOperationException();

            var activeSeason = res2.Result.Seasons.OrderByDescending(x => x.StartDate).FirstOrDefault();
            using (var context = _leagueContextFactory.CreateDbContext())
            {
                if (activeSeason != null)
                {
                    var existingSeason = context.Seasons.FirstOrDefault(ss => ss.LeagoL1Key == activeSeason.LeagoL1Key);
                    if (existingSeason == null)
                    {
                        existingSeason = new Season()
                        {
                            LeagoL1Key = activeSeason.LeagoL1Key,
                            Title = activeSeason.Title,
                        };

                        context.Seasons.Add(existingSeason);
                        context.SaveChanges();
                    }
                }
            }
        }

        public async Task SyncMatchesForRound(int round)
        {
            var activeSeason = await _leagueDataService.RunQueryAsync(new GetActiveSeasonQuery());
            if (activeSeason == null)
                return;

            var getMatchesRes = await _leagoService.GetMatches(new GetMatchesInDto()
            {
                RoundKey = round,
                TournamentKey = activeSeason.LeagoL2Key,
                MatchesCount = 100
            });

            if (getMatchesRes != null)
            {
                await _leagueDataService.ExecuteAsync(
                    new AddOrUpdateMatchesCommand()
                    {
                        ToUpdate = getMatchesRes.Matches,
                        SeasonId = activeSeason.Id,
                        Round = round
                    });
            }
        }

        public async Task SyncMatches()
        {
            try
            {
                for (int ii = 0; ii < 5; ii++)
                    await SyncMatchesForRound(ii + 1);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        
    }
}