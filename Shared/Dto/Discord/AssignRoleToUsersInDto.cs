using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dto.Discord
{
    public class AssignRoleToUsersInDto
    {
        public int RoleId { get; set; }
        public string[] Users { get; set; } = Array.Empty<string>();
    }
}
