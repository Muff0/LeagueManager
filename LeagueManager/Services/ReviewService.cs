using Data.Commands;
using Data.Model;
using Data.Queries;
using Data;
using Shared.Enum;
using Shared.Dto;
using Data.Commands.Review;

namespace LeagueManager.Services
{
    public class ReviewService
    {

        private readonly LeagueDataService _leagueDataService;

        private readonly int[][] _reviewPattern = [[1, 3],[2, 4], [3, 5],[1,4],[2,5]];


        public ReviewService(LeagueDataService leagueDataService)
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
                    for (var i = 0; i < player.Reviews.Count; i++)
                        updateList.Add(new ReviewDto()
                        {
                            Id = player.Reviews.ElementAt(i).Id,
                            Round = i + 1
                        });

                else if (player.Reviews.Count == 2)
                {
                    for (var i = 0; i < player.Reviews.Count; i++)
                        updateList.Add(new ReviewDto()
                        {
                            Id = player.Reviews.ElementAt(i).Id,
                            Round = _reviewPattern[currentPattern][i]
                        });
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

        private int GetReviewCount(PlayerParticipationTier tier)
        {
            if (tier == PlayerParticipationTier.DojoTier2)
                return 2;
            if (tier == PlayerParticipationTier.DojoTier3)
                return 5;
            return 0;
        }
    }
}