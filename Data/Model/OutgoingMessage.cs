using System.ComponentModel.DataAnnotations;
using Shared.Enum;

namespace Data.Model
{
    public class OutgoingMessage
    {
        [Key]
        public int Id { get; set; }

        public QueueStatus Status { get; set; }

        public string Type { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
        public int CorrelationId { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public DateTime ProcessedAtUtc { get; set; }

        public int Retries { get; set; }
    }
}