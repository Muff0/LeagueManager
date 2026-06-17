using System.ComponentModel.DataAnnotations;

namespace Data.Model
{
    
    public class JobRegistry
    {
        public int Id { get; set; }
        [MaxLength(30)]
        public string JobType { get; set; } = null!;
        public bool IsEnabled { get; set; } = true;
        public string? SettingsJson { get; set; }
        public DateTime? LastRunAt { get; set; }
        public DateTime SettingsUpdatedAt { get; set; }
    }
}