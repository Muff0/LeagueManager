using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.Enum;

namespace Data.Model;

public class GameAnalysis
{
    [Key()]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Sgf { get; set; }  = string.Empty;
    public string StateUrl { get; set; } = string.Empty;
    public int MatchId { get; set; }
    public QueueStatus Status { get; set; } = QueueStatus.Pending;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime ProcessedAtUtc { get; set; }
}