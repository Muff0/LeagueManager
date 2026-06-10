namespace Shared.Dto.Discord;

public class PostReviewThreadInDto
{
    public ReviewDto Review { get; set; } = new();
    public TeacherDto Teacher { get; set; } = new();
    public MatchDto Match { get; set; } = new();
}