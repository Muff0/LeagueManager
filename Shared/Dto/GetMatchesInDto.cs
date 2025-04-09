using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dto
{
    public class GetMatchesInDto
    {
        public int RoundKey { get; set; }
        public string TournamentKey { get; set; } = string.Empty;
        public int MatchesOffset { get; set; } = 0;
        public int MatchesCount { get; set; } = 0;
    }
}
