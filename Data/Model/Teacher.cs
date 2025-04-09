using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model
{
    public class Teacher
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public float Rate { get; set; } = 0;
        public int MaxRank { get; set; }
        public int Rank { get; set; }

        public string MailAddress { get; set; } = string.Empty;
        public string DiscordHandle { get; set; } = string.Empty;

        public ICollection<Review>? Reviews { get; set; }

    }
}
