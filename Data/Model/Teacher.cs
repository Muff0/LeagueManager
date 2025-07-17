using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.Enum;

namespace Data.Model
{
    public class Teacher
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public float Rate { get; set; } = 0;
        public PlayerRank MaxRank { get; set; }
        public PlayerRank Rank { get; set; }

        public string MailAddress { get; set; } = string.Empty;
        public ulong? DiscordId { get; set; }

        public ICollection<Review>? Reviews { get; set; }
    }
}