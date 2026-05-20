using Data;
using Data.Commands;
using Data.Commands.Match;
using Data.Commands.Review;
using Data.Model;
using Data.Queries;
using Shared;
using Shared.Dto;
using Shared.Enum;

namespace LeagueManager.Services
{
    public class ReviewService : ServiceBase
    {
        private readonly Dictionary<PlayerParticipationTier, int> _participationTiers =
            new()
            {
                { PlayerParticipationTier.DojoTier2, 1 },
                { PlayerParticipationTier.DojoTier3, 3 },
                { PlayerParticipationTier.DojoTier4, 5 }
            };
        
        public ReviewService(LeagueDataService leagueDataService,
            ILogger<ReviewService> logger) : base(logger)
        {
            _leagueDataService = leagueDataService;
        }

        private async Task<int[]> GetReviewsPerRound(int seasonId)
        {
            var resGetReviews = await _leagueDataService.RunQueryAsync(new GetReviewsQuery());

            int[] revPerRound = new int[6];

            for (int ii = 1; ii <= 6; ii++)
                revPerRound[ii] = resGetReviews.Count(rr => rr.Round == ii);
            return revPerRound;
        }

        /// <summary>
        /// Returns round with least reviews for which the player doesn't already have one scheduled
        /// </summary>
        private int GetRoundWithLeastReviews(int[] revPerRound, int[] playerScheduledRounds)
        {
            int bestRound = -1;
            int bestCount = int.MaxValue;

            for (int round = 1; round < revPerRound.Length; round++)
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
        
        public async Task AssignRoundsToReviews()
        {

            var activeSeason = await _leagueDataService.RunQueryAsync(new GetActiveSeasonQuery());
            var resGetPlayers = await _leagueDataService.RunQueryAsync(new GetPlayerSeasonsQuery()
            {
                SeasonId = activeSeason.Id,
                IncludeReviews = true,
                ParticipationTiers = _participationTiers.Keys.ToArray()
            });

            // Keeps track of reviews per round, to spread them out, round 0 included to keep it simple
            int[] roundReviewCount = await GetReviewsPerRound(activeSeason.Id);
            
            var updateList = new List<ReviewDto>();

            foreach (var player in resGetPlayers)
            {
                foreach (Review current in player.Reviews)
                {
                    if (current.Round > 0)
                        //already assigned
                        continue;

                    int round = GetRoundWithLeastReviews(roundReviewCount,
                        player.Reviews.Select(rr => rr.Round).ToArray());
                    
                    current.Round = round;
                    updateList.Add(new ReviewDto()
                    {
                        Id = current.Id,
                        Round = round
                    });
                }
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
                var reviewCount = _participationTiers.GetValueOrDefault(player.ParticipationTier);
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


        public async Task AddReviewToPlayer(PlayerDto playerDto)
        {

            var season = await _leagueDataService.RunQueryAsync(new GetActiveSeasonQuery());
            await _leagueDataService.ExecuteAsync(new AddReviewToPlayerCommand() { PlayerId = playerDto.Id ?? 0, SeasonId = season.Id });
        }
    }
}