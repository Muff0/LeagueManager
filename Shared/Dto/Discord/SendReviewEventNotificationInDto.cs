namespace Shared.Dto.Discord
{
    public class SendReviewEventNotificationInDto
    {
        public DateTime DateTimeUTC { get; set; }
        public TeacherDto Teacher { get; set; } = new TeacherDto();
        public MatchDto[] Reviews { get; set; } = [];
    }
}
