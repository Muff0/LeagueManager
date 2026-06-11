namespace Shared.Dto;

public class Streak
{
    public PlayerDto PlayerDto { get; set; }
    public bool IsOngoing { get; set; }
    public MatchDto Start { get; set; }
}