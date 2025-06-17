using LeagoClient;
using Shared;
using Shared.Dto;
using Shared.Enum;

namespace LeagoService
{
    public class LeagoMainService : ServiceBase
    {
        private readonly AccountClient _accountClient;
        private readonly ArenaMembersClient _arenaMembersClient;
        private readonly ArenasClient _arenasClient;
        private readonly EventsClient _eventsClient;
        private readonly HealthClient _healthClient;
        private readonly LeaguesClient _leaguesClient;
        private readonly MatchesClient _matchesClient;
        private readonly PaymentsClient _paymentsClient;
        private readonly ProfilesClient _profilesClient;
        private readonly RoundsClient _roundsClient;
        private readonly TournamentsClient _tournamentsClient;
        private readonly UsersClient _usersClient;

        public LeagoMainService(AccountClient accountClient,
            ArenaMembersClient arenaMembersClient,
            ArenasClient arenasClient,
            EventsClient eventsClient,
            HealthClient healthClient,
            LeaguesClient leaguesClient,
            MatchesClient matchesClient,
            PaymentsClient paymentsClient,
            ProfilesClient profilesClient,
            RoundsClient roundsClient,
            TournamentsClient tournamentsClient,
            UsersClient usersClient)
        {
            _accountClient = accountClient;
            _arenaMembersClient = arenaMembersClient;
            _arenasClient = arenasClient;
            _eventsClient = eventsClient;
            _healthClient = healthClient;
            _leaguesClient = leaguesClient;
            _matchesClient = matchesClient;
            _paymentsClient = paymentsClient;
            _profilesClient = profilesClient;
            _roundsClient = roundsClient;
            _tournamentsClient = tournamentsClient;
            _usersClient = usersClient;
        }

        public Task GetHealth()
        {
            return _healthClient.HealthAsync();
        }

        public async Task<GetTournamentOutDto> GetTournament(GetTournamentInDto inDto)
        {
            var res = new GetTournamentOutDto();
            try
            {
                var ires = await _tournamentsClient.GetTournamentAsync(inDto.TournamentName);

                res.Result = new TournamentDto()
                {
                    LeagoTournamentId = inDto.TournamentName,
                    Name = ires.Title
                };
            }
            catch
            {
            }
            return res;
        }

        public async Task<GetMatchesOutDto> GetMatches(GetMatchesInDto inDto)
        {
            var res = new GetMatchesOutDto();

            try
            {
                var rres = await _roundsClient.ListRoundsAsync(inDto.TournamentKey);

                var targetround = rres.FirstOrDefault(rr => rr.Status == RoundStatus.MatchesInProgress);

                var mres = await _roundsClient.ListRoundMatchesAsync(inDto.TournamentKey, targetround.Ordinal, inDto.MatchesOffset, inDto.MatchesCount);

                res.Matches = mres.Select(mm => new MatchDto()
                {
                    LeagoKey = mm.Key,
                    ScheduleTime = mm.WallTime.GetValueOrDefault().UtcDateTime,
                    GameLink = mm.OnlineGameUrl,
                    MatchSetLevel = mm.MatchSetLevel,
                    Players = mm.Players.Select(pp => new PlayerMatchDto()
                    {
                        Player = new PlayerDto()
                        {
                            FirstName = pp.GivenName,
                            LastName = pp.FamilyName,
                            LeagoKey = pp.Key,
                            LeagoMemberId = pp.MemberId,
                            OGSHandle = pp.OnlineHandle,
                            Rank = (PlayerRank)pp.RankId
                        },
                        Color = (pp.Color.Equals("black", StringComparison.OrdinalIgnoreCase)) ? PlayerColor.Black : PlayerColor.White,
                        HasConfirmed = pp.IsMatchTimeAccepted,
                        HasForfeited = false,
                        Outcome = (Shared.Enum.PlayerMatchOutcome)pp.Outcome
                    }).ToArray()
                }).ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return res;
        }

        public async Task<GetPlayersOutDto> GetPlayers(GetPlayersInDto inDto)
        {
            var res = new GetPlayersOutDto();
            try
            {
                var ires = await _eventsClient.ListTournamentsAsync(inDto.TournamentKey, false);

                var pres = await _tournamentsClient.ListTournamentPlayersAsync(ires.FirstOrDefault()?.Key ?? "");

                res.Players = pres.Select(
                    p => new PlayerDto()
                    {
                        FirstName = p.GivenName ?? "",
                        LastName = p.FamilyName ?? "",
                        LeagoMemberId = p.MemberId ?? "",
                        LeagoKey = p.Key ?? "",
                        Rank = (PlayerRank)p.RankId,
                        OGSHandle = p.OnlineHandle ?? ""
                    }).ToArray();
            }
            catch
            {
                // k
            }
            return res;
        }

        public async Task<GetEventsOutDto> GetEvents(GetEventsInDto inDto)
        {
            var res = new GetEventsOutDto();
            try
            {
                var ires = await _leaguesClient.GetLeaguePageAsync(0, 10);

                res.Events = ires.Items.Select(i => new EventDto()
                {
                    Name = i.Name
                }).ToArray();
            }
            catch
            {
            }
            return res;
        }

        public async Task<GetLeagueOutDto> GetLeague(GetLeagueInDto inDto)
        {
            var res = new GetLeagueOutDto();
            try
            {
                var sres = await _arenasClient.GetLeagueSeasonsPageAsync(inDto.ArenaKey, inDto.LeagueKey, "", false, true, "", 0, 10);

                var league = new Shared.Dto.LeagueDto()
                {
                    LeagueKey = inDto.LeagueKey
                };

                league.Seasons = sres.Items.Select(ii => new Shared.Dto.SeasonDto()
                {
                    LeagoL1Key = ii.Key,
                    Title = ii.Title,
                    IsActive = ii.IsComplete,
                    StartDate = ii.StartLocalTime
                }).ToArray();

                res.Result = league;
            }
            catch
            {
            }
            return res;
        }

        public async Task<GetArenaOutDto> GetArena(GetArenaInDto inDto)
        {
            var res = new GetArenaOutDto();
            try
            {
                var ires = await _arenasClient.GetArenaAsync(inDto.ArenaKey);

                res.Result = new ArenaDto()
                {
                };
            }
            catch
            {
            }
            return res;
        }

        public async Task<GetProfileOutDto> GetProfile(GetProfileInDto inDto)
        {
            var res = new GetProfileOutDto();
            try
            {
                var ires = await _profilesClient.GetPublicProfileAsync(inDto.ProfileKey);
                var rres = await _arenasClient.GetMemberAsync(inDto.ArenaKey, inDto.ArenaKey + "-" + inDto.ProfileKey);
                res.Timezone = ires.Timezone;
                res.Email = rres.Email;
                res.DiscordHandle = ires.Discord;
            }
            catch
            {
            }
            return res;
        }
    }
}