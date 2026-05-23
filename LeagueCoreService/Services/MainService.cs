using Data;
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
        private readonly PollSchedulerService _pollSchedulerService;

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


        public async Task<Data.Model.Match[]> GetUpcomingMatches()
        {
            var query = new Data.Queries.GetMatchesByTimeQuery()
            {
                InlcudePlayers = true,
                IncludeCompleted = false,
                IncludeNotConfirmed = false,
                TimeFrom = DateTime.Now.ToUniversalTime(),
                TimeTo = DateTime.Now.AddMinutes(360).ToUniversalTime(),
                Count = 5
            };

            var res = await _leagueDataService.RunQueryAsync(query);

            return res.ToArray();
        }

        public async Task SendUpcomingMatchesNotification()
        {
            try
            {
                var resm = await GetUpcomingMatches();

                if (resm.Length == 0)
                    return;

                await _discordService.SendUpcomingMatchesNotification(new SendUpcomingMatchesNotificationInDto()
                {
                    Matches = resm.Select(mm => mm.ToMatchDto()).ToArray()
                });
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
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
                using (var context = _leagueContextFactory.CreateDbContext())
                {
                    var toAdd = new List<Data.Model.Match>();
                    foreach (var currentMatch in getMatchesRes.Matches)
                    {
                        if (currentMatch.Players == null)
                            throw new InvalidOperationException("Players is null");

                        var existingMatch = context.Matches.Include(mm => mm.PlayerMatches)
                            .ThenInclude(pm => pm.Player)
                            .FirstOrDefault(mm => mm.LeagoKey == currentMatch.LeagoKey);

                        if (existingMatch == null)
                        {
                            var newMatch = new Data.Model.Match()
                            {
                                LeagoKey = currentMatch.LeagoKey,
                                SeasonId = activeSeason.Id,
                                Round = round,
                                MatchUrl = currentMatch.GameLink ?? "",
                                GameTimeUTC = currentMatch.ScheduleTime.GetValueOrDefault().ToUniversalTime(),
                                PlayerMatches = new List<PlayerMatch>()
                            };

                            foreach (PlayerMatchDto playerMatch in currentMatch.Players)
                            {
                                if (playerMatch?.Player == null)
                                    continue;

                                var existingPlayer = context.Players.FirstOrDefault(pp => pp.LeagoKey == playerMatch.Player.LeagoKey);

                                if (existingPlayer == null)
                                    continue;

                                var newPlayerMatch = new PlayerMatch()
                                {
                                    Color = playerMatch.Color,
                                    PlayerId = existingPlayer.Id,
                                    HasConfirmed = playerMatch.HasConfirmed,
                                    Outcome = playerMatch.Outcome,
                                };

                                newMatch.PlayerMatches.Add(newPlayerMatch);
                            }

                            toAdd.Add(newMatch);
                        }
                        else
                        {
                            existingMatch.GameTimeUTC = currentMatch.ScheduleTime.GetValueOrDefault().ToUniversalTime();
                            existingMatch.MatchUrl = currentMatch.GameLink ?? "";

                            existingMatch.IsComplete = currentMatch.Players.Any(pl => pl.Outcome != Shared.Enum.PlayerMatchOutcome.NotReported);

                            foreach (PlayerMatchDto playerMatch in currentMatch.Players)
                            {
                                var existingPlayerMatch = existingMatch.PlayerMatches?.FirstOrDefault(pm => pm.Player?.LeagoKey == playerMatch.Player?.LeagoKey);

                                if (existingPlayerMatch == null)
                                    continue;

                                existingPlayerMatch.HasConfirmed = playerMatch.HasConfirmed;
                                existingPlayerMatch.Outcome = playerMatch.Outcome;
                                existingPlayerMatch.Color = playerMatch.Color;
                            }
                        }
                    }

                    await context.AddRangeAsync(toAdd);
                    await context.SaveChangesAsync();
                }
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

        public async Task CleanupQueue()
        {
            await _queueDataService.ExecuteAsync(new DeleteOldCommandMessagesCommand());
        }

        public async Task PostNextDiscordPoll()
        {
            var nextPoll = await _queueDataService.RunQueryAsync(
                new GetNextPollQuery());

            if (nextPoll == null)
                return;

            var payload = nextPoll.GetPayload<DiscordPoll>();

            if (payload == null)
                return;

            await _discordService.PostPoll(payload);
            await _queueDataService.ExecuteAsync(new SetPollStatusCommand()
            {
                PollId = nextPoll.Id,
                NewStatus = QueueStatus.Completed,
                UpdateProcessedTime = true
            });

        }
        
    }
}