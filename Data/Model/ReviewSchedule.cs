using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Model
{
    public class ReviewSchedule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ReviewId { get; set; }

        public DateTime UTCSchedule { get; set; }

        public ulong DiscordEventId { get; set; }
        public Review? Review { get; set; }
    }
}
