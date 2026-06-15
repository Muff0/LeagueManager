using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Shared.Enum;

namespace Data.Model;

public class GameAnalysis
{
    [Key()]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Sgf { get; set; }
    public int MatchId { get; set; }
    public QueueStatus Status { get; set; } = QueueStatus.Pending;
    public DateTime DateTime { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime ProcessedAtUtc { get; set; }
}