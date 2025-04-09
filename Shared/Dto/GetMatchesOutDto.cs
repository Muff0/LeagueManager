using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dto
{
    public class GetMatchesOutDto
    {
        public MatchDto[] Matches { get; set; } = Array.Empty<MatchDto>();
    }
}
