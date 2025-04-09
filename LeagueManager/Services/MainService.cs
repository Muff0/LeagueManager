using System.Drawing;
using Data;
using Data.Model;
using Discord;
using LeagoClient;
using LeagoService;
using Microsoft.Extensions.Options;
using Shared.Dto;
using Shared.Dto.Discord;
using Shared.Settings;

namespace LeagueManager.Services
{
    public class MainService
    {
        private readonly LeagoMainService _leagoService;

        private readonly LeagueContext _context;
        private readonly IOptions<LeagoSettings> _leagoOptions;
        private readonly LeagueDataService _dataService;
        private readonly DiscordService _discordService;

        public MainService(LeagoMainService leagoService, 
            IOptions<LeagoSettings> leagoOptions, 
            LeagueContext leagueContext, 
            LeagueDataService dataService,
            DiscordService discordService)
        {
            _context = leagueContext;
            _leagoService = leagoService;
            _leagoOptions = leagoOptions;
            _dataService = dataService;
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
                TimeFrom = DateTime.Now,
                TimeTo = DateTime.Now.AddMinutes(24000)
            };

            var res = await _dataService.RunQueryAsync(query);

            return res.ToArray();
        }

        public async Task SendNotification()
        {
            var resm = await GetUpcomingMatches();

            await _discordService.SendUpcomingMatchesNotification(new SendUpcomingMatchesNotificationInDto()
            {
                Matches = resm.Select(mm => new MatchDto()
                {
                   
                }).ToArray()
            });
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

            if (activeSeason != null)
            {

                var existingSeason = _context.Seasons.FirstOrDefault(ss => ss.LeagoL1Key == activeSeason.LeagoL1Key);
                if (existingSeason == null)
                {
                    existingSeason = new Season()
                    {
                        LeagoL1Key = activeSeason.LeagoL1Key,
                        Title = activeSeason.Title,
                    };

                    _context.Seasons.Add(existingSeason);
                    _context.SaveChanges();
                }
            }
        }

        public SeasonDto? GetActiveSeason()
        {
            var existingSeason = _context.Seasons.FirstOrDefault(ss => ss.IsActive);

            if (existingSeason == null)
                return null;

            return new SeasonDto()
            {
                Id = existingSeason.Id,
                Title  = existingSeason.Title,
                LeagoL1Key = existingSeason.LeagoL1Key,
                LeagoL2Key = existingSeason.LeagoL2Key,
                IsActive = existingSeason.IsActive
            };
        }

        public async Task SyncMatches()
        {
            var activeSeason = GetActiveSeason();
            if (activeSeason == null)
                return;

            var getMatchesRes = await _leagoService.GetMatches(new GetMatchesInDto()
            {
                RoundKey = 2,
                TournamentKey = activeSeason.LeagoL2Key,
                MatchesCount = 100
            });

            if (getMatchesRes != null)
            {
                var toAdd = new List<Data.Model.Match>();
                foreach (var currentMatch in getMatchesRes.Matches)
                {
                    if (currentMatch.Players == null)
                        throw new InvalidOperationException("Players is null");

                    var existingMatch = _context.Matches.FirstOrDefault(mm => mm.LeagoKey == currentMatch.LeagoKey);

                    if (existingMatch == null)
                    {
                        var newMatch = new Data.Model.Match()
                        {
                            LeagoKey = currentMatch.LeagoKey,
                            SeasonId = activeSeason.Id,
                            Round = 2,
                            Link = currentMatch.GameLink,
                            GameTimeUTC = currentMatch.ScheduleTime?.UtcDateTime,
                            PlayerMatches = new List<PlayerMatch>()
                        };

                        foreach (PlayerMatchDto playerMatch in currentMatch.Players)
                        {

                            if (playerMatch?.Player == null)
                                continue;

                            var existingPlayer = _context.Players.FirstOrDefault(pp => pp.LeagoKey == playerMatch.Player.LeagoKey);

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
                        existingMatch.GameTimeUTC = currentMatch.ScheduleTime?.UtcDateTime;
                        existingMatch.Link = currentMatch.GameLink;
                        existingMatch.IsComplete = currentMatch.IsPlayed;

                        foreach (PlayerMatchDto playerMatch in currentMatch.Players)
                        {
                            var existingPlayerMatch = existingMatch.PlayerMatches?.FirstOrDefault(pm => pm.Player?.LeagoKey == playerMatch.Player?.LeagoKey);

                            if (existingPlayerMatch == null)
                                continue;

                            existingPlayerMatch.HasConfirmed = playerMatch.HasConfirmed;
                            existingPlayerMatch.Outcome = playerMatch.Outcome;
                        }
                    }

                }

                await _context.AddRangeAsync(toAdd);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdatePlayers()
        {

            var activeSeason = GetActiveSeason();
            if (activeSeason == null) 
                return;

            var getPlayersRes = await _leagoService.GetPlayers(new Shared.Dto.GetPlayersInDto()
            {
                TournamentKey = activeSeason.LeagoL1Key
            });


            List<Data.Model.Player> toAdd = new List<Data.Model.Player>();

                foreach (var player in getPlayersRes.Players)
                {
                    var existingPlayer = _context.Players.FirstOrDefault(pl => pl.FirstName == player.FirstName && pl.LastName == player.LastName);

                    if (existingPlayer == null)
                    {
                        existingPlayer = new Data.Model.Player()
                        {
                            FirstName = player.FirstName,
                            LastName = player.LastName,
                            OGSHandle = player.OGSHandle,
                            LeagoMemberId = player.LeagoMemberId,
                            LeagoKey = player.LeagoKey,
                            Rank = player.Rank,
                        };
                        toAdd.Add(existingPlayer);
                    }
                    else
                    {
                        existingPlayer.Rank = player.Rank;
                        existingPlayer.OGSHandle = player.OGSHandle;
                        existingPlayer.LeagoMemberId = player.LeagoMemberId;
                        existingPlayer.LeagoKey = player.LeagoKey;
                    }

                        var currentPlayerSeason = existingPlayer.PlayerSeasons.FirstOrDefault(ps => ps.SeasonId == activeSeason.Id && ps.PlayerId == existingPlayer.Id);

                    if (currentPlayerSeason == null)
                    {
                        existingPlayer.PlayerSeasons.Add(
                            new PlayerSeason()
                            {
                                SeasonId = activeSeason.Id,
                                ParticipationTier = Shared.Enum.PlayerParticipationTier.Registered
                            });
                    }
                }

                _context.AddRange(toAdd);

                _context.SaveChanges();
        
        }
    }
}
