using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Enum;

namespace Data.Model
{
    public class PlayerSeason
    {
        public int PlayerId { get; set; }
        public int SeasonId { get; set; }

        public Season? Season { get; set; }

        public Player? Player { get; set; }


        public ICollection<Match>? Matches { get; set; } 

        public PlayerParticipationTier ParticipationTier { get; set; }
    }
}
