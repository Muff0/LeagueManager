using Shared.Dto.Discord;
using Microsoft.Extensions.Hosting;

using NetCord.Hosting.Gateway;
using NetCord.Rest;
using Shared.Settings;
using NetCord;
using Microsoft.Extensions.Options;

namespace Discord
{
    public class DiscordService
    {
        private readonly RestClient _client;
        private readonly IOptions<DiscordSettings> _settings;


        public DiscordService(IOptions<DiscordSettings> settings)
        {
            _settings = settings;
            var to = new BotToken(_settings.Value.Token);
            _client = new RestClient(to);
        }

        public async Task SendUpcomingMatchesNotification(SendUpcomingMatchesNotificationInDto inDto)
        {
            var msg = new MessageProperties()
                .WithContent("Well, it wasn't too hard.");

            try
            {

                await _client.SendMessageAsync(_settings.Value.TestChannelId, msg);
            }
            catch (Exception ex)
            {
                return;
            }
        }

    }
}
