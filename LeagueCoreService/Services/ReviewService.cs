using Data;
using Data.Commands;
using Data.Commands.Match;
using Data.Commands.Review;
using Data.Model;
using Data.Queries;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using Shared;
using Shared.Dto;
using Shared.Enum;

namespace LeagueCoreService.Services
{
    public class ReviewService : ServiceBase
    {
        public ReviewService(LeagueDataService leagueDataService
            , ILogger<ReviewService> logger) : base(logger)
        {
            _leagueDataService = leagueDataService;
        }

        public async Task AssignRoundsToReviews()
        {

            var activeSeason = await _leagueDataService.RunQueryAsync(new GetActiveSeasonQuery());
            var resGetPlayers = await _leagueDataService.RunQueryAsync(new GetPlayerSeasonsQuery()
            {
                SeasonId = activeSeason.Id,
                IncludeReviews = true,
                ParticipationTiers = [PlayerParticipationTier.DojoTier2, PlayerParticipationTier.DojoTier3]
            });

            var updateList = new List<ReviewDto>();
            int currentPattern = 0;

            foreach (var player in resGetPlayers)
            {
                if (player.Reviews.Count == 5)
                {
                    for (var i = 0; i < player.Reviews.Count; i++)
                    {
                        var current = player.Reviews.FirstOrDefault(rr => rr.Round == i + 1);
                        if (current == null)
                        {
                            var toUpdate = player.Reviews.FirstOrDefault(rr => rr.Round == 0);
                            if (toUpdate != null)
                            {
                                toUpdate.Round = i + 1;
                                updateList.Add(new ReviewDto()
                                {
                                    Id = player.Reviews.ElementAt(i).Id,
                                    Round = i + 1
                                });
                            }
                        }
                    }
                }

                else if (player.Reviews.Count == 2)
                {
                    for (var i = 0; i < player.Reviews.Count; i++)
                    {
                        int[] pattern = _reviewPattern[currentPattern];
                        var current = player.Reviews.FirstOrDefault(rr => rr.Round == pattern[i]);
                        if (current == null)
                        {
                            var toUpdate = player.Reviews.FirstOrDefault(rr => rr.Round == 0);
                            if (toUpdate != null)
                            {
                                toUpdate.Round = pattern[i];
                                updateList.Add(new ReviewDto()
                                {
                                    Id = player.Reviews.ElementAt(i).Id,
                                    Round = pattern[i]
                                });
                            }
                        }
                    }
                    currentPattern++;
                    if (currentPattern == _reviewPattern.Length)
                        currentPattern = 0;
                }
                else
                    continue;
            }
            await _leagueDataService.ExecuteAsync(new UpdateReviewsCommand()
            {
                Reviews = updateList.ToArray()
            });
        }

        public async Task BuildReviews()
        {
            var activeSeason = await _leagueDataService.RunQueryAsync(new GetActiveSeasonQuery());
            var resGetPlayers = await _leagueDataService.RunQueryAsync(
                new GetPlayerSeasonsQuery()
                {
                    IncludePlayer = true,
                    IncludeReviews = true,
                    SeasonId = activeSeason.Id,
                    ParticipationTiers = [PlayerParticipationTier.DojoTier2, PlayerParticipationTier.DojoTier3]
                });

            var toAdd = new List<Review>();

            foreach (var player in resGetPlayers)
            {
                var reviewCount = GetReviewCount(player.ParticipationTier);
                int revDiff = reviewCount - player.Reviews.Count;

                if (revDiff <= 0)
                    continue;

                for (int i = 0; i < revDiff; i++)
                {
                    var review = new Review();
                    review.OwnerPlayerId = player.PlayerId;
                    review.SeasonId = player.SeasonId;
                    toAdd.Add(review);
                }
            }
            _leagueDataService.Execute(new InsertEntitiesCommand<LeagueContext, Review>(toAdd));
        }

        public async Task<ReviewScheduleDto[]> GetReviewSchedule()
        {
            var activeSeason = await _leagueDataService.RunQueryAsync(new GetActiveSeasonQuery());
            var resGetReviews = await _leagueDataService.RunQueryAsync(new GetReviewsQuery()
            {
                SeasonId = activeSeason.Id,
                IncludeOwner = true
            });

            var groupedRev = resGetReviews.GroupBy(re => re.OwnerPlayerId);
            var res = new List<ReviewScheduleDto>();

            foreach (var group in groupedRev)
            {
                var owner = group.First().OwnerPlayer;

                res.Add(new ReviewScheduleDto()
                {
                    DiscordId = owner!.DiscordId,
                    OwnerEmail = owner!.EmailAddress,
                    OwnerName = owner.FirstName + " " + owner.LastName,
                    ReviewedRounds = group.Select(gr => gr.Round).Order().ToArray(),
                });
            }
            return res.ToArray();
        }

        /// <summary>
        /// Links reviews to the corresponding Matches, if they exist
        /// </summary>
        public async Task LinkExistingReviewMatches()
        {
            await _leagueDataService.ExecuteAsync(
                new LinkExistingReviewMatchesCommand());
        }

        public async Task SetReviewStatus(SetReviewStatusInDto setReviewStatusInDto)
        {
            await _leagueDataService.ExecuteAsync(new SetReviewStatusCommand()
            {
                ReviewList = setReviewStatusInDto.Reviews,
                NewStatus = setReviewStatusInDto.NewStatus
            });
        }

        public async Task UnlinkMatch(IEnumerable<ReviewDto> reviews, bool deleteMatch = false)
        {
            if (!reviews.Any()) return;

            List<int> toDelete = new List<int>();

            foreach (var rev in reviews)
            {
                if (deleteMatch)
                    toDelete.Add((int)rev.MatchId!);
                rev.MatchId = null;
            }

            await _leagueDataService.ExecuteAsync(new UpdateReviewsCommand()
            {
                Reviews = reviews.ToArray()
            });

            if (deleteMatch && toDelete.Count > 0)
                await _leagueDataService.ExecuteAsync(new DeleteMatchesCommand()
                {
                    MatchIds = toDelete.ToArray()
                });
        }

        internal async Task AssignTeacherToReviews(IEnumerable<ReviewDto> reviews, int teacherId)
        {
            await _leagueDataService.ExecuteAsync(new AssignTeacherToReviewsCommand()
            {
                TeacherId = teacherId,
                ReviewIds = reviews.Select(re => re.Id).ToArray()
            });

        }

        private readonly LeagueDataService _leagueDataService;

        private readonly int[][] _reviewPattern = [[1, 3], [2, 4], [3, 5], [1, 4], [2, 5]];
        private int GetReviewCount(PlayerParticipationTier tier)
        {
            if (tier == PlayerParticipationTier.DojoTier2)
                return 2;
            if (tier == PlayerParticipationTier.DojoTier3)
                return 5;
            return 0;
        }

        public async Task AddReviewToPlayer(PlayerDto playerDto)
        {

            var season = await _leagueDataService.RunQueryAsync(new GetActiveSeasonQuery());
            await _leagueDataService.ExecuteAsync(new AddReviewToPlayerCommand() { PlayerId = playerDto.Id ?? 0, SeasonId = season.Id });
        }
    }
}