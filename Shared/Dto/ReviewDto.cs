using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Enum;

namespace Shared.Dto
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int Round { get; set; }
        public int? TeacherId { get; set; }
        public ReviewStatus ReviewStatus { get; set; }
        public int? MatchId { get; set; }
        public string? ReviewUrl { get; set; }
    }
}
