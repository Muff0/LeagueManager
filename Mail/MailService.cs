using Microsoft.Extensions.Options;
using Shared.Settings;

namespace Mail
{
    public class MailService
    {
        private readonly IOptions<MailSettings> _settings;

        public MailService(IOptions<MailSettings> settings)
        {
            _settings = settings;
        }
    }
}