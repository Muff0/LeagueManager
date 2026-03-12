using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Commands.Match
{
    public class DeleteMatchesCommand : Command<LeagueContext>
    {
        public int[] MatchIds { get; set; } = [];

        protected override void RunAction(LeagueContext context)
        {
            base.RunAction(context);

            foreach (var id in MatchIds)
            {
                var existing = context.Matches.FirstOrDefault(mm => mm.Id == id);

                if (existing != null)
                {
                    context.Matches.Remove(existing);
                }
            }
        }
    }
}
