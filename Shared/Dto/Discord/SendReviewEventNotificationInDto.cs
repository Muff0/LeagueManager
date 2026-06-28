namespace Shared.Dto.Discord;

public class SendReviewEventNotificationInDto
{
    public DateTime DateTimeUtc { get; set; }
    public TeacherDto Teacher { get; set; } = new();
    public MatchDto[] Reviews { get; set; } = [];
}