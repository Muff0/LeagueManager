namespace Shared.Dto
{
    public class GetMatchesInDto
    {
        public int RoundKey { get; set; }
        public string TournamentKey { get; set; } = string.Empty;
        public int MatchesOffset { get; set; } = 0;
        public int MatchesCount { get; set; } = 0;
    }
}