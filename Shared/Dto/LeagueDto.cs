namespace Shared.Dto
{
    public class LeagueDto
    {
        public string LeagueKey { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public SeasonDto[] Seasons { get; set; } = Array.Empty<SeasonDto>();
    }
}