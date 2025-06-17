using Shared.Enum;

namespace Shared.Dto
{
    public class PlayerDto
    {
        public int? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? EmailAddress { get; set; }
        public string? DiscordHandle { get; set; }
        public string? OGSHandle { get; set; }
        public string? LeagoMemberId { get; set; }
        public PlayerRank Rank { get; set; } = PlayerRank.MinValue;
        public string? LeagoKey { get; set; }
        public int? GoMagicUserId { get; set; }
        public ulong? DiscordId { get; set; }
    }
}