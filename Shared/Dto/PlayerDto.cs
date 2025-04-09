using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dto
{
    public class PlayerDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string EmailAddress { get; set; } = "";
        public string DiscordHandle { get; set; } = "";
        public string OGSHandle { get; set; } = "";
        public string LeagoMemberId { get; set; } = "";
        public int Rank { get; set; }
        public string LeagoKey { get; set; } = string.Empty;
    }
}
