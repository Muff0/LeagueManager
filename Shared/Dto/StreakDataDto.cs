namespace Shared.Dto;

public class StreakDataDto
{
    public List<Streak> TopStreak { get; set; } = [];
    public int TopStreakCount { get; set; }
    public bool IsTopOngoing { get; set; }
    public bool WasTopBroken { get; set; }
    public List<Streak> RunnerUpStreak { get; set; } = [];
    public  int RunnerUpCount { get; set; }
    public List<Streak> LongestOngoingStreak { get; set; } = [];
    public int LongestOngoingCount { get; set; }
}