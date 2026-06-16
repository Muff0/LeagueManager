using Data;
using Data.Commands;
using Data.Commands.Match;
using Data.Commands.Review;
using Data.Model;
using Data.Queries;
using LeagueManager.Extensions;
using LeagueManager.ViewModel;
using Shared;
using Shared.Dto;
using Shared.Enum;

namespace LeagueManager.Services;

public class ReviewService(
    LeagueDataService leagueDataService,
    ILogger<ReviewService> logger) : ServiceBase(logger)
{
    private readonly Dictionary<PlayerParticipationTier, int> _participationTiers =
        new()
        {
            { PlayerParticipationTier.DojoTier2, 1 },
            { PlayerParticipationTier.DojoTier3, 3 },
            { PlayerParticipationTier.DojoTier4, 5 }
        };


    private async Task<int[]> GetReviewsPerRound(int seasonId)
    {
        var resGetReviews = await leagueDataService.RunQueryAsync(new GetReviewsQuery
        {
            SeasonId = seasonId
        });

        var revPerRound = new int[6];

        for (var ii = 1; ii < 6; ii++)
            revPerRound[ii] = resGetReviews.Count(rr => rr.Round == ii);
        return revPerRound;
    }


    public async Task<List<ReviewViewModel>> GetReviewsToPost(int? startIndex, int? count,
        CancellationToken cancellationToken)
    {
        try
        {
            var season = await leagueDataService.TakeFirstAsync(new GetActiveSeasonQuery());
            var res = await leagueDataService.RunQueryAsync(new GetReviewsQuery
            {
                IncludeMatch = true,
                IncludeOwner = true,
                IncludeTeacher = true,
                IncludePlayerSeasons = true,
                Round = [1, 2, 3, 4, 5],
                SeasonId = season.Id,
                Status = [ReviewStatus.Allocated],
                MatchQueryMode = GetReviewsQuery.ReviewMatchQueryMode.WithMatchOnly,
                Count = count ?? 0,
                StartIndex = startIndex ?? 0
            });
            return res.Select(rev => rev.ToViewModel()).ToList();
        }
        catch (Exception ex)
        {
            HandleException(ex);
            return new List<ReviewViewModel>();
        }
    }

    /// <summary>
    ///     Returns round with least reviews for which the player doesn't already have one scheduled
    /// </summary>
    private int GetRoundWithLeastReviews(int[] revPerRound, int[] playerScheduledRounds)
    {
        var bestRound = -1;
        var bestCount = int.MaxValue;

        for (var round = 1; round < revPerRound.Length; round++)
        {
            if (playerScheduledRounds.Contains(round))
                continue;

            if (revPerRound[round] < bestCount)
            {
                bestCount = revPerRound[round];
                bestRound = round;
            }
        }

        return bestRound;
    }


    public async Task<List<ReviewViewModel>> GetOverviewReviews(int? startIndex, int? count,
        CancellationToken cancellationToken)
    {
        try
        {
            var res = await leagueDataService.RunQueryAsync(new GetReviewsQuery
            {
                IncludeMatch = true,
                IncludeOwner = true,
                IncludeTeacher = true,
                IncludePlayerSeasons = true,
                Count = count ?? 0,
                StartIndex = startIndex ?? 0
            });
            return res.Select(rev => rev.ToViewModel())
                .OrderBy(rev => rev.SeasonName)
                .ThenBy(rev => rev.Round)
                .ThenBy(rev => rev.OwnerName)
                .ToList();
        }
        catch (Exception ex)
        {
            HandleException(ex);
            return new List<ReviewViewModel>();
        }
    }


    public async Task AssignRoundsToReviews()
    {
        var activeSeason = await leagueDataService.TakeFirstAsync(new GetActiveSeasonQuery());
        var resGetPlayers = await leagueDataService.RunQueryAsync(new GetPlayerSeasonsQuery
        {
            SeasonId = activeSeason.Id,
            IncludeReviews = true,
            ParticipationTiers = _participationTiers.Keys.ToArray()
        });

        // Keeps track of reviews per round, to spread them out, round 0 included to keep it simple
        var roundReviewCount = await GetReviewsPerRound(activeSeason.Id);

        var updateList = new List<ReviewDto>();

        foreach (var player in resGetPlayers)
        foreach (var current in player.Reviews)
        {
            if (current.Round > 0)
                //already assigned
                continue;

            var round = GetRoundWithLeastReviews(roundReviewCount,
                player.Reviews.Select(rr => rr.Round).ToArray());

            current.Round = round;
            updateList.Add(new ReviewDto
            {
                Id = current.Id,
                Round = round
            });
            roundReviewCount[round]++;
        }

        await leagueDataService.ExecuteAsync(new UpdateReviewsCommand
        {
            Reviews = updateList.ToArray()
        });
    }

    public async Task BuildMissingReviews()
    {
        var activeSeason = await leagueDataService.TakeFirstAsync(new GetActiveSeasonQuery());
        var resGetPlayers = await leagueDataService.RunQueryAsync(
            new GetPlayerSeasonsQuery
            {
                IncludePlayer = true,
                IncludeReviews = true,
                SeasonId = activeSeason.Id,
                ParticipationTiers =
                [
                    PlayerParticipationTier.DojoTier2, PlayerParticipationTier.DojoTier3,
                    PlayerParticipationTier.DojoTier4
                ]
            });

        var toAdd = new List<Review>();

        foreach (var player in resGetPlayers)
        {
            var reviewCount = _participationTiers.GetValueOrDefault(player.ParticipationTier);
            var revDiff = reviewCount - player.Reviews.Count;

            if (revDiff <= 0)
                continue;

            for (var i = 0; i < revDiff; i++)
            {
                var review = new Review();
                review.OwnerPlayerId = player.PlayerId;
                review.SeasonId = player.SeasonId;
                toAdd.Add(review);
            }
        }

        leagueDataService.Execute(new InsertEntitiesCommand<LeagueContext, Review>(toAdd));
    }

    public async Task<ReviewScheduleDto[]> GetReviewSchedule()
    {
        var activeSeason = await leagueDataService.TakeFirstAsync(new GetActiveSeasonQuery());
        var resGetReviews = await leagueDataService.RunQueryAsync(new GetReviewsQuery
        {
            SeasonId = activeSeason.Id,
            IncludeOwner = true
        });

        var groupedRev = resGetReviews.GroupBy(re => re.OwnerPlayerId);
        var res = new List<ReviewScheduleDto>();

        foreach (var group in groupedRev)
        {
            var owner = group.First().OwnerPlayer;

            res.Add(new ReviewScheduleDto
            {
                DiscordId = owner!.DiscordId,
                OwnerEmail = owner!.EmailAddress,
                OwnerName = owner.FirstName + " " + owner.LastName,
                ReviewedRounds = group.Select(gr => gr.Round).Order().ToArray()
            });
        }

        return res.ToArray();
    }

    /// <summary>
    ///     Links reviews to the corresponding Matches, if they exist
    /// </summary>
    public async Task LinkExistingReviewMatches()
    {
        await leagueDataService.ExecuteAsync(
            new LinkExistingReviewMatchesCommand());
    }

    public async Task SetReviewStatus(SetReviewStatusInDto setReviewStatusInDto)
    {
        await leagueDataService.ExecuteAsync(new SetReviewStatusCommand
        {
            ReviewList = setReviewStatusInDto.Reviews,
            NewStatus = setReviewStatusInDto.NewStatus
        });
    }

    public async Task UnlinkMatch(IEnumerable<ReviewDto> reviews, bool deleteMatch = false)
    {
        if (!reviews.Any()) return;

        var toDelete = new List<int>();

        foreach (var rev in reviews)
        {
            if (deleteMatch)
                toDelete.Add((int)rev.MatchId!);
            rev.MatchId = null;
        }

        await leagueDataService.ExecuteAsync(new UpdateReviewsCommand
        {
            Reviews = reviews.ToArray()
        });

        if (deleteMatch && toDelete.Count > 0)
            await leagueDataService.ExecuteAsync(new DeleteMatchesCommand
            {
                MatchIds = toDelete.ToArray()
            });
    }

    internal async Task AssignTeacherToReviews(IEnumerable<ReviewDto> reviews, int teacherId)
    {
        await leagueDataService.ExecuteAsync(new AssignTeacherToReviewsCommand
        {
            TeacherId = teacherId,
            ReviewIds = reviews.Select(re => re.Id).ToArray()
        });
    }


    public async Task<List<ReviewViewModel>> GetReviewsToSchedule(int? startIndex, int? count,
        CancellationToken cancellationToken, bool includeNoMatch)
    {
        try
        {
            var season = await leagueDataService.TakeFirstAsync(new GetActiveSeasonQuery());
            var res = await leagueDataService.RunQueryAsync(new GetReviewsQuery
            {
                IncludeMatch = true,
                IncludeOwner = true,
                IncludeTeacher = true,
                Round = [1, 2, 3, 4, 5],
                SeasonId = season.Id,
                Status = [ReviewStatus.Planned],
                MatchQueryMode = includeNoMatch
                    ? GetReviewsQuery.ReviewMatchQueryMode.AllReviews
                    : GetReviewsQuery.ReviewMatchQueryMode.WithMatchOnly,
                Count = count ?? 0,
                StartIndex = startIndex ?? 0
            });
            return res.Select(rev => rev.ToViewModel()).ToList();
        }
        catch (Exception ex)
        {
            HandleException(ex);
            return new List<ReviewViewModel>();
        }
    }


    public async Task BuildReviews()
    {
        try
        {
            await BuildMissingReviews();
            await AssignRoundsToReviews();
        }
        catch (Exception ex)
        {
            HandleException(ex);
        }
    }

    public async Task SetDuplicateReview(IEnumerable<ReviewViewModel> reviews)
    {
        try
        {
            await SetReviewStatus(new SetReviewStatusInDto
            {
                Reviews = reviews.Select(re => re.ToReviewDto()),
                NewStatus = ReviewStatus.Duplicate
            });
        }
        catch (Exception e)
        {
            HandleException(e);
        }
    }

    public async Task MoveToAllocated(IEnumerable<ReviewViewModel> reviews)
    {
        try
        {
            await SetReviewStatus(new SetReviewStatusInDto
            {
                Reviews = reviews.Select(re => re.ToReviewDto()),
                NewStatus = ReviewStatus.Allocated
            });
        }
        catch (Exception e)
        {
            HandleException(e);
        }
    }

    public async Task AddReviewToPlayer(PlayerDto playerDto)
    {
        var season = await leagueDataService.TakeFirstAsync(new GetActiveSeasonQuery());
        await leagueDataService.ExecuteAsync(new AddReviewToPlayerCommand
            { PlayerId = playerDto.Id ?? 0, SeasonId = season.Id });
    }
}