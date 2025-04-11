using System.Drawing;
using Data;
using Data.Model;
using Discord;
using LeagoClient;
using LeagoService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.Dto;
using Shared.Dto.Discord;
using Shared.Settings;

namespace LeagueManager.Services
{
    public class MainService
    {
        private readonly QueueContext _queueContext;

        public MainService(QueueContext queueContext)
        {
            _queueContext = queueContext;
        }

        public async Task SendNotification()
        {
            var command = new CommandMessage()
            {
                CreatedAtUtc = DateTime.UtcNow,
                Type = "SendNotification",
                Payload = ""
            };

            await _queueContext.CommandQueue.AddAsync(command);
            await _queueContext.SaveChangesAsync();
        }

        public async Task SyncMatches()
        {
            var command = new CommandMessage()
            {
                CreatedAtUtc = DateTime.UtcNow,
                Type = "SyncMatches",
                Payload = ""
            };

            await _queueContext.CommandQueue.AddAsync(command);
            await _queueContext.SaveChangesAsync();
        }

        public async Task UpdatePlayers()
        {
            var command = new CommandMessage()
            {
                CreatedAtUtc = DateTime.UtcNow,
                Type = "UpdatePlayers",
                Payload = ""
            };

            await _queueContext.CommandQueue.AddAsync(command);
            await _queueContext.SaveChangesAsync();
        }
    }
}
