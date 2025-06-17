namespace Shared.Dto.Discord
{
    public class RemoveRoleFromUsersInDto
    {
        public ulong RoleId { get; set; }
        public ulong[] UserIds { get; set; } = [];
    }
}