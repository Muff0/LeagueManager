namespace Shared.Dto.Discord
{
    public class AssignRoleToUsersInDto
    {
        public int RoleId { get; set; }
        public string[] Users { get; set; } = Array.Empty<string>();
    }
}