namespace Shared.Dto
{
    public class GetProfileOutDto
    {
        public string Email { get; set; } = string.Empty;
        public string OGSHandle { get; set; } = string.Empty;
        public string DiscordHandle { get; set; } = string.Empty;
        public string Timezone { get; set; } = string.Empty;
    }
}