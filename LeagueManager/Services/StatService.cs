using Data;
using Data.Model;
using Data.Queries;
using Shared.Dto;
using Shared.Enum;

namespace LeagueManager.Services;

public class StatService(LeagueDataService leagueDataService)
{
    public async Task<StatsDto> GetStats()
    {
        
        var players = await leagueDataService.RunQueryAsync(new GetPlayersQuery()
        {
            IncludePlayerMatches = true,
            IncludeMatches = true
        });

        var res = new StatsDto();
        res.RivalryData = await GetRivalryData();
        res.StreakData = await GetStreakData(players);
        return res;
    }

    private async Task<RivalryDataDto> GetRivalryData()
    {
        var matches = await leagueDataService.RunQueryAsync(new GetMatchesQuery()
        {
            IncludePlayers = true

        });
        var canonList = matches
            .GroupBy(mm =>new 
            {
                Min = Math.Min(mm.PlayerMatches.First().PlayerId,mm.PlayerMatches.Last().PlayerId),
                Max = Math.Max(mm.PlayerMatches.First().PlayerId,mm.PlayerMatches.Last().PlayerId)
            });
        int maxplayed = 0;
        Player[] rivals = [];
        Match[] history = [];
        
        foreach (var pair in canonList)
        {
            // some malformed matches, must skip
            if (pair.Key.Min == pair.Key.Max)
                continue;
            var count = pair.Count();
            if (count > maxplayed)
            {
                maxplayed = count;
                rivals = pair.First().PlayerMatches.Select(pm => pm.Player).ToArray();
                history = pair.ToArray();
            }
        }
        
        var res = new RivalryDataDto();
        return res;

    }

    public async Task<StreakDataDto> GetStreakData(IList<Player> players)
    {

        var resStreak = new StreakDataDto();

        foreach (var player in players)
        {
            var matches = player.PlayerMatches.OrderBy(pm => pm.Match.SeasonId)
                .ThenBy(pm => pm.Match.Round);
            int currentStreak = 0;
            int topStreak = 0;
            PlayerMatch topStart = null;
            PlayerMatch currentStart = null;
            bool newStreak = true;
            bool isOngoing = false;
            foreach (var playerMatch in matches)
            {
                if (newStreak)
                {
                    currentStreak = 0;
                    currentStart = playerMatch;
                    newStreak = false;
                }
                
                if (playerMatch.Outcome == PlayerMatchOutcome.Win)
                    currentStreak++;
                else
                {
                    if (currentStreak >= topStreak)
                    {
                        topStreak = currentStreak;
                        topStart = currentStart;
                        
                    }

                    newStreak = true;
                }
            }

            if (currentStreak == 0)
                continue;
            
            if (currentStreak >= topStreak)
            {
                topStreak = currentStreak;
                topStart = currentStart;
                isOngoing = true;
            }
            
            if (topStreak > resStreak.TopStreakCount)
            {
                resStreak.TopStreakCount = topStreak;
                resStreak.IsTopOngoing = isOngoing;
                resStreak.TopStreak.Clear();
                    resStreak.TopStreak.Add(new Streak()
                    {
                        PlayerDto = player.ToPlayerDto(),
                        IsOngoing = isOngoing,
                        Start = topStart.Match.ToMatchDto()
                    });
            }
            else if (topStreak == resStreak.TopStreakCount)
            {
                resStreak.TopStreak.Add(new Streak()
                {
                    PlayerDto = player.ToPlayerDto(),
                    IsOngoing = isOngoing,
                    Start = topStart.Match.ToMatchDto()
                });
            }
            
            else if (topStreak > resStreak.RunnerUpCount)
            {
                resStreak.RunnerUpCount = topStreak;
                resStreak.RunnerUpStreak.Clear();
                resStreak.RunnerUpStreak.Add(new Streak()
                {
                    PlayerDto = player.ToPlayerDto(),
                    IsOngoing = isOngoing,
                    Start = topStart.Match.ToMatchDto()
                });
            }
            else if (topStreak == resStreak.RunnerUpCount)
            {
                resStreak.RunnerUpStreak.Add(new Streak()
                {
                    PlayerDto = player.ToPlayerDto(),
                    IsOngoing = isOngoing,
                    Start = topStart.Match.ToMatchDto()
                });
            }

            if (isOngoing)
            {
                if (topStreak > resStreak.LongestOngoingCount)
                {
                    resStreak.LongestOngoingCount = topStreak;
                    resStreak.LongestOngoingStreak.Clear();
                        resStreak.LongestOngoingStreak.Add(new Streak()
                        {
                            PlayerDto = player.ToPlayerDto(),
                            IsOngoing = isOngoing,
                            Start = topStart.Match.ToMatchDto()
                        });
                }
                else if (topStreak == resStreak.LongestOngoingCount)
                {
                    resStreak.LongestOngoingStreak.Add(new Streak()
                    {
                        PlayerDto = player.ToPlayerDto(),
                        IsOngoing = isOngoing,
                        Start = topStart.Match.ToMatchDto()
                    });
                }
            }
        }
        return resStreak;
    }
}