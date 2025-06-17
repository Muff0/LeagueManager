using Microsoft.Extensions.Options;
using Shared;
using Shared.Settings;

namespace Mail
{
    public class MailService : ServiceBase
    {
        private readonly IOptions<MailSettings> _settings;

        public MailService(IOptions<MailSettings> settings)
        {
            _settings = settings;
        }
    }
}