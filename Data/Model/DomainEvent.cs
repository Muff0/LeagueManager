using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Enum;

namespace Data.Model
{
    public class DomainEvent
    {

        [Key]
        public int Id { get; set; }

        public QueueStatus Status { get; set; }

        public string Type { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;

        public DateTime CreatedAtUtc { get; set; }
        public DateTime ProcessedAtUtc { get; set; }
        public int CorrelationId { get; set; }

        public int Retries { get; set; }
    }
}
