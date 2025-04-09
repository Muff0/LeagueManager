using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model
{
    public class Season
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string LeagoL1Key { get; set; } = string.Empty;
        public string LeagoL2Key { get; set; } = string.Empty;
        public ICollection<PlayerSeason> PlayerSeasons { get; set; } = new List<PlayerSeason>();

        public ICollection<Match>? Matches { get; set; }
        public bool IsActive { get; set; }
    }
}
