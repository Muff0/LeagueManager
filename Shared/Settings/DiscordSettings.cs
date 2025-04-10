using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Settings
{
    public class DiscordSettings
    {
        public string AppId { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty ;
        public string Token { get; set; } = string.Empty;
        public ulong TestChannelId { get; set; } 
        public ulong MatchAnnouncementChannelId { get; set; }
        public ulong MatchAnnouncementRoleId { get; set; }
    }
}
