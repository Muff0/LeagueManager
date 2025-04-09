using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dto.Discord
{
    public class SendUpcomingMatchesNotificationInDto
    {
        public MatchDto[] Matches { get; set; } = Array.Empty<MatchDto>();
    }
}
