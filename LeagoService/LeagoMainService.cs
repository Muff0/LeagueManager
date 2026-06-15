using LeagoClient;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Dto;
using Shared.Enum;
using LeagueDto = Shared.Dto.LeagueDto;
using PlayerMatchOutcome = Shared.Enum.PlayerMatchOutcome;

namespace LeagoService;

public class LeagoMainService : ServiceBase
{
    private readonly ArenasClient _arenasClient;
    private readonly EventsClient _eventsClient;
    private readonly HealthClient _healthClient;
    private readonly LeaguesClient _leaguesClient;
    private readonly ProfilesClient _profilesClient;
    private readonly RoundsClient _roundsClient;
    private readonly TournamentsClient _tournamentsClient;
    public LeagoMainService(
        ArenasClient arenasClient,
        EventsClient eventsClient,
        HealthClient healthClient,
        LeaguesClient leaguesClient,
        ProfilesClient profilesClient,
        RoundsClient roundsClient,
        TournamentsClient tournamentsClient,
        ILogger<LeagoMainService> logger) : base(logger)
    {
        _arenasClient = arenasClient;
        _eventsClient = eventsClient;
        _healthClient = healthClient;
        _leaguesClient = leaguesClient;
        _profilesClient = profilesClient;
        _roundsClient = roundsClient;
        _tournamentsClient = tournamentsClient;
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

            res.Result = new TournamentDto
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

    private int TryGetIdFromLeagueMatchUrl(string matchUrl)
    {
        var idString =  matchUrl.Split('/').Last();
        if (int.TryParse(idString, out var id))
            return id;
        return 0;
    }
    
    public async Task<GetMatchesOutDto> GetMatches(GetMatchesInDto inDto)
    {
        var res = new GetMatchesOutDto();

        var mres = await _roundsClient.ListRoundMatchesAsync(inDto.TournamentKey, inDto.RoundKey, inDto.MatchesOffset,
            inDto.MatchesCount);

        res.Matches = mres.Select(mm => new MatchDto
        {
            LeagoKey = mm.Key,
            ScheduleTime = mm.WallTime.GetValueOrDefault().UtcDateTime,
            OgsLeagueMatchId = TryGetIdFromLeagueMatchUrl(mm.OnlineGameUrl),
            Players = mm.Players.Select(pp => new PlayerMatchDto
            {
                Player = new PlayerDto
                {
                    FirstName = pp.GivenName,
                    LastName = pp.FamilyName,
                    LeagoKey = pp.Key,
                    LeagoMemberId = pp.MemberId,
                    OGSHandle = pp.OnlineHandle,
                    Rank = (PlayerRank)pp.RankId
                },
                Color = pp.Color.Equals("black", StringComparison.OrdinalIgnoreCase)
                    ? PlayerColor.Black
                    : PlayerColor.White,
                HasConfirmed = pp.IsMatchTimeAccepted,
                HasForfeited = false,
                Outcome = (PlayerMatchOutcome)pp.Outcome
            }).ToArray()
        }).ToArray();

        return res;
    }

    public async Task<GetPlayersOutDto> GetPlayers(GetPlayersInDto inDto)
    {
        var res = new GetPlayersOutDto();
        try
        {
            var ires = await _eventsClient.ListTournamentsAsync(inDto.TournamentKey, false);

            var pres = await _tournamentsClient.ListTournamentPlayersAsync(ires.FirstOrDefault()?.Key ?? "");

            res.Players = pres.Select(p => new PlayerDto
            {
                FirstName = p.GivenName ?? "",
                LastName = p.FamilyName ?? "",
                LeagoMemberId = p.MemberId ?? "",
                LeagoKey = p.Key ?? "",
                Rank = (PlayerRank)p.RankId,
                OGSHandle = p.OnlineHandle ?? ""
            }).ToArray();
        }
        catch(Exception e)
        {
            HandleException(e);
        }

        return res;
    }

    public async Task<GetEventsOutDto> GetEvents(GetEventsInDto inDto)
    {
        var res = new GetEventsOutDto();
        try
        {
            var ires = await _leaguesClient.GetLeaguePageAsync(0, 10);

            res.Events = ires.Items.Select(i => new EventDto
            {
                Name = i.Name
            }).ToArray();
        }
        catch(Exception e)
        {
            HandleException(e);
        }

        return res;
    }

    public async Task<GetLeagueOutDto> GetLeague(GetLeagueInDto inDto)
    {
        var res = new GetLeagueOutDto();
        try
        {
            var sres = await _arenasClient.GetLeagueSeasonsPageAsync(inDto.ArenaKey, inDto.LeagueKey, "", false, true,
                "", 0, 10);

            var league = new LeagueDto
            {
                LeagueKey = inDto.LeagueKey
            };

            league.Seasons = sres.Items.Select(ii => new SeasonDto
            {
                LeagoL1Key = ii.Key,
                Title = ii.Title,
                IsActive = ii.IsComplete,
                StartDate = ii.StartLocalTime
            }).ToArray();

            res.Result = league;
        }
        catch(Exception e)
        {
            HandleException(e);
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
        catch(Exception e)
        {
            HandleException(e);
        }

        return res;
    }
}