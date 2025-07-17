using Data;
using Data.Model;
using Discord;
using LeagoService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared;
using Shared.Dto;
using Shared.Dto.Discord;
using Shared.Settings;

namespace LeagueCoreService.Services
{
    public class MainService : ServiceBase
    {
        private readonly LeagoMainService _leagoService;

        private readonly IDbContextFactory<LeagueContext> _leagueContextFactory;
        private readonly IOptions<LeagoSettings> _leagoOptions;
        private readonly LeagueDataService _leagueDataService;
        private readonly DiscordService _discordService;

        public MainService(LeagoMainService leagoService,
            IOptions<LeagoSettings> leagoOptions,
            IDbContextFactory<LeagueContext> leagueContextFactory,
            LeagueDataService dataService,
            DiscordService discordService)
        {
            _leagueContextFactory = leagueContextFactory;
            _leagoService = leagoService;
            _leagoOptions = leagoOptions;
            _leagueDataService = dataService;
            _discordService = discordService;
        }

        public async Task<string[]> GetTournamentsAsync()
        {
            var res = await _leagoService.GetEvents(new Shared.Dto.GetEventsInDto());

            return res.Events.Select(i => i.Name).ToArray();
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

        public SeasonDto? GetActiveSeason()
        {
            using (var context = _leagueContextFactory.CreateDbContext())
            {
                var existingSeason = context.Seasons.FirstOrDefault(ss => ss.IsActive);

                if (existingSeason == null)
                    return null;

                return new SeasonDto()
                {
                    Id = existingSeason.Id,
                    Title = existingSeason.Title,
                    LeagoL1Key = existingSeason.LeagoL1Key,
                    LeagoL2Key = existingSeason.LeagoL2Key,
                    IsActive = existingSeason.IsActive
                };
            }
        }

        public async Task SyncMatchesForRound(int round)
        {
            var activeSeason = GetActiveSeason();
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
                            existingMatch.IsComplete = currentMatch.IsPlayed;

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

    }
}