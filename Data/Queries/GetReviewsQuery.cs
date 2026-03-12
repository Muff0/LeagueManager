using Data.Model;
using Microsoft.EntityFrameworkCore;
using Shared.Enum;

namespace Data.Queries
{
    public class GetReviewsQuery : Query<LeagueContext, Review>
    {
        public enum ReviewMatchQueryMode
        {
            AllReviews,
            WithoutMatchOnly,
            WithMatchOnly
        }

        public bool IncludeOwner { get; set; } = false;
        public bool IncludeMatch { get; set; } = false;
        /// <summary>
        /// If a value is provided the query filters the items that have/have not the ReviewUrl field not blank
        /// </summary>
        public bool? HasReviewUrl { get; set; } = null;
        public bool IncludeTeacher { get; set; } = false;
        public bool IncludePlayerSeasons { get; set; } = false;
        public int? SeasonId { get; set; }
        public int[]? ExcludeSeasonIds { get; set; }
        public ReviewStatus[]? Status { get; set; }
        public int[]? Round { get; set; }

        public ReviewMatchQueryMode MatchQueryMode { get; set; } = ReviewMatchQueryMode.AllReviews;
        protected override IQueryable<Review> BuildQuery(LeagueContext context)
        {
            var query = base.BuildQuery(context);

            if (IncludeOwner)
                query = query.Include(re => re.OwnerPlayer);

            if (IncludeMatch)
                query = query.Include(re => re.Match)
                    .ThenInclude(mm => mm!.PlayerMatches)
                    .ThenInclude(pm => pm.Player);

            if (IncludeTeacher)
                query = query.Include(re => re.Teacher);

            if (ExcludeSeasonIds != null)
                query = query.Where(re => !ExcludeSeasonIds.Contains(re.SeasonId));

            if (IncludePlayerSeasons)
                query = query.Include(re => re.PlayerSeason)
                    .ThenInclude(ps => ps.Season);

            if (SeasonId != null)
                query = query.Where(re => re.SeasonId == SeasonId);

            if (Status != null && Status.Length > 0)
                query = query.Where(re => Status.Contains(re.ReviewStatus));

            if (Round != null && Round.Length > 0)
                query = query.Where(re => Round.Contains(re.Round));

            if (HasReviewUrl != null)
                query = query.Where(re => HasReviewUrl == !string.IsNullOrWhiteSpace(re.ReviewUrl));

            if (MatchQueryMode == ReviewMatchQueryMode.WithoutMatchOnly)
                query = query.Where(re => re.MatchId == null);
            else if (MatchQueryMode == ReviewMatchQueryMode.WithMatchOnly)
                query = query.Where(re => re.MatchId != null);

            return query;
        }
    }
}