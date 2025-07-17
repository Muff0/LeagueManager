using Shared.Enum;

namespace Shared.Dto
{
    public class TeacherDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public PlayerRank Rank { get; set; } = PlayerRank.MinValue;
        public ulong DiscordId { get; set; }
    }
}
