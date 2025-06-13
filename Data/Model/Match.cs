using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Data.Model
{
    public class Match
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public ICollection<PlayerMatch> PlayerMatches { get; set; } = new List<PlayerMatch>();
        public string Link { get; set; } = string.Empty;

        public bool IsComplete { get; set; } = false;

        public int Round { get; set; }
        public int SeasonId { get; set; }

        public Season? Season { get; set; }

        public bool NotificationSent { get; set; } = false;

        public DateTime? GameTimeUTC { get; set; }

        [Required]
        public string LeagoKey { get; set; } = string.Empty;
    }
}
