using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Model;
using Microsoft.EntityFrameworkCore;

namespace Data.Commands.Review
{

    /// <summary>
    /// Links reviews to the corresponding Matches, if they exist
    /// </summary>
    public class LinkExistingReviewMatchesCommand : Command<LeagueContext>
    {
        protected override void RunAction(LeagueContext context)
        {
            base.RunAction(context);

            // Just take the unassigned ones
            var reviews = context.Reviews.Where(re => re.MatchId == null).ToList();

            foreach (var review in reviews)
            {
                var match = context.PlayerMatches.Include(pm => pm.Match)
                    .FirstOrDefault(pm => pm.PlayerId == review.OwnerPlayerId
                        && pm.Match!.Round == review.Round
                        && pm.Match!.SeasonId == review.SeasonId);

                if (match != null) 
                    review.MatchId = match.MatchId;
            }
        }
    }
}
