using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model
{
    public class Player
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public required string FirstName { get; set; }
        [Required]
        public required string LastName { get; set; }
        public string EmailAddress { get; set; } = String.Empty;
        public string DiscordHandle { get; set; } = String.Empty;
        public string OGSHandle { get; set; } = String.Empty;
        public string LeagoMemberId { get; set; } = String.Empty;
        public int Rank { get; set; }



        public ICollection<PlayerSeason> PlayerSeasons { get; set; } = new List<PlayerSeason>();
        public string LeagoKey { get; set; } = string.Empty;
        public ICollection<PlayerMatch>? PlayerMatches { get; set; }
    }
}
