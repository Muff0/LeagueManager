using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dto
{
    public class ReviewScheduleDto
    {
        public string OwnerName { get; set; } = string.Empty;
        public int[] ReviewedRounds { get; set; } = [];
        public ulong? DiscordId { get; set; }
        public string OwnerEmail { get; set; } = string.Empty;
    }
}
