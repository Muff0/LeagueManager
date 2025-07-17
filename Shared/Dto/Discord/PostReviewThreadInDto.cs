namespace Shared.Dto.Discord
{
    public class PostReviewThreadInDto
    {
        public ReviewDto Review { get; set; } = new ReviewDto();
        public TeacherDto Teacher { get; set; } = new TeacherDto();
        public MatchDto Match { get; set; } = new MatchDto();
    }
}
