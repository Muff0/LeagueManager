namespace Shared.Dto.Discord
{
    public class SendUpcomingMatchesNotificationInDto
    {
        public MatchDto[] Matches { get; set; } = Array.Empty<MatchDto>();
    }
}