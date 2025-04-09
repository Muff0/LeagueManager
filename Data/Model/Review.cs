using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Enum;

namespace Data.Model
{
    public class Review
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? MatchId { get; set; }

        public Match? Match { get; set; }

        public ReviewStatus ReviewStatus { get; set; } = ReviewStatus.Planned;

        public int OwnerPlayerId { get; set; }

        public Player? OwnerPlayer { get; set; } 

        public int? TeacherId { get; set; }

        public Teacher? Teacher { get; set; }

        public int Round { get; set; } = 0;
    }
}
