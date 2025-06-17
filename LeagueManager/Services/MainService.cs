using System.Data;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Data;
using Data.Commands;
using Data.Commands.Player;
using Data.Commands.Queue;
using Data.Model;
using Data.Queries;
using Discord;
using LeagoService;
using Mail;
using Microsoft.Extensions.Options;
using Shared.Dto;
using Shared.Dto.Discord;
using Shared.Enum;
using Shared.Settings;

namespace LeagueManager.Services
{
    public class MainService
    {
        private readonly QueueDataService _queueDataService;
        private readonly LeagueDataService _leagueDataService;
        private readonly LeagoMainService _leagoService;
        private readonly IOptions<LeagoSettings> _leagoOptions;
        private readonly DiscordService _discordService;
        private readonly ReviewService _reviewService;
        private readonly MailService _mailService;

        public MainService(QueueDataService queueDataService,
            LeagueDataService leagueDataService,
            LeagoMainService leagoService,
            IOptions<LeagoSettings> leagoOptions,
            DiscordService discordService,
            ReviewService reviewService,
            MailService mailService)
        {
            _queueDataService = queueDataService;
            _leagueDataService = leagueDataService;
            _leagoService = leagoService;
            _leagoOptions = leagoOptions;
            _discordService = discordService;
            _reviewService = reviewService;
            _mailService = mailService;
        }

        public async Task<DataTable> BuildOrderDataTable(Stream csvStream)
        {
            var table = new DataTable();

            using var reader = new StreamReader(csvStream);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                DetectDelimiter = true,
                HasHeaderRecord = true,
            });

            await csv.ReadAsync();
            csv.ReadHeader();
            foreach (var header in csv.HeaderRecord)
            {
                table.Columns.Add(header);
            }

            while (await csv.ReadAsync())
            {
                var row = table.NewRow();
                foreach (DataColumn column in table.Columns)
                {
                    row[column.ColumnName] = csv.GetField(column.DataType, column.ColumnName) ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }

            return table;
        }

        protected PlayerParticipationTier GetParticipationTier(string product)
        {
            if (product == "Go Magic League - Participation + Lectures")
                return PlayerParticipationTier.DojoTier1;
            if (product == "Go Magic League - Lectures + 2 reviews")
                return PlayerParticipationTier.DojoTier2;
            if (product == "Go Magic League - Lectures + 5 reviews")
                return PlayerParticipationTier.DojoTier3;
            return PlayerParticipationTier.None;
        }

        public async Task<bool> UploadOrdersFile(Stream content)
        {
            using var contentStream = new StreamReader(content);

            var table = await BuildOrderDataTable(content);

            var payments = ParsePaymentData(table);

            await ProcessPaymentData(payments);

            return true;
        }

        public async Task BuildReviews()
        {
            await _reviewService.BuildReviews();
            await _reviewService.AssignRoundsToReviews();
        }

        public async Task FetchMissingDiscordIds()
        {
            try
            {
                var players = await _leagueDataService.RunQueryAsync(new GetPlayersQuery());

                var missingIds = players.Where(pl => pl.DiscordHandle != null
                    && pl.DiscordHandle != ""
                    && pl.DiscordId == null);

                var updateList = new List<PlayerDto>();

                foreach (var player in missingIds)
                {
                    var id = await _discordService.GetDiscordUserId(player.DiscordHandle);
                    if (id != null)
                        updateList.Add(new PlayerDto()
                        {
                            Id = player.Id,
                            DiscordId = id
                        });
                }

                await _leagueDataService.ExecuteAsync(new UpdatePlayersDataCommand() { Players = updateList.ToArray() });
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void HandleException(Exception ex)
        {
            //lol
            return;
        }

        public async Task UpdateDiscordPlayerRole()
        {
            try
            {
                var activeSeason = await _leagueDataService.RunQueryAsync(new GetActiveSeasonQuery());
                var currentPlayers = await _leagueDataService.RunQueryAsync(new GetPlayerSeasonsQuery()
                {
                    IncludePlayer = true,
                    SeasonId = activeSeason.Id,
                });

                await _discordService.UpdatePlayerRole(new UpdatePlayerRoleInDto()
                {
                    CurrentPlayers = currentPlayers.Select(ps => ps.Player)
                        .Where(pl => pl!.DiscordId != null)
                        .Select(pl => pl!.DiscordId)
                        .Cast<ulong>()
                        .ToArray()
                });
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        protected async Task ProcessPaymentData(PaymentDataDto[] payments)
        {
            var cutoffDate = new DateTime(2025, 05, 10);

            var season = await _leagueDataService.RunQueryAsync(new GetActiveSeasonQuery() { IncludePlayerSeasons = true });
            var players = await _leagueDataService.RunQueryAsync(new GetPlayersQuery());

            var noMatch = new List<PaymentDataDto>();
            var noReg = new List<PaymentDataDto>();
            var toUpdate = new List<PlayerRegistrationDto>();
            var updateGoMagicIdList = new List<PlayerDto>();

            foreach (var payment in payments)
            {
                if (payment.DateTime < cutoffDate)
                    continue;

                var player = TryFindPlayer(players, payment);

                if (player == null)
                {
                    noMatch.Add(payment);
                    continue;
                }

                if (player.GoMagicUserId == 0)
                    updateGoMagicIdList.Add(
                        new PlayerDto()
                        {
                            Id = player.Id,
                            GoMagicUserId = payment.UserId,
                        });

                var currentPlayerSeason = season.PlayerSeasons.FirstOrDefault(ps => ps.PlayerId == player.Id);

                if (currentPlayerSeason == null)
                {
                    noReg.Add(payment);
                    continue;
                }

                var partTier = GetParticipationTier(payment.Product);

                if (currentPlayerSeason.PaymentStatus == PlayerPaymentStatus.Refunded || currentPlayerSeason.PaymentStatus == PlayerPaymentStatus.Cancelled)
                    continue;

                if (currentPlayerSeason.PaymentStatus == PlayerPaymentStatus.None
                    || currentPlayerSeason.ParticipationTier != partTier)
                    toUpdate.Add(new PlayerRegistrationDto()
                    {
                        DateTime = payment.DateTime,
                        PlayerId = player.Id,
                        PlayerParticipationTier = partTier,
                        SeasonId = season.Id,
                        PlayerPaymentStatus = PlayerPaymentStatus.Paid
                    });
            }

            try
            {
                _leagueDataService.Execute(new UpdatePlayerSeasons() { PlayerRegistrations = toUpdate.ToArray() });
                _leagueDataService.Execute(new UpdatePlayersDataCommand() { Players = updateGoMagicIdList.ToArray() });

                string noRegstr = string.Join(";", noReg.Select(pd => pd.UserEmail));
            }
            catch
            {
                ;
            }
        }

        public async Task<PlayerDto[]> GetMissingPayments()
        {
            var resGetMP = await _leagueDataService.RunQueryAsync(new GetMissingPaymentsQuery() { SeasonId = 2 });

            return resGetMP.Select(pl => new PlayerDto()
            {
                FirstName = pl.FirstName,
                LastName = pl.LastName,
                EmailAddress = pl.EmailAddress,
                DiscordHandle = pl.DiscordHandle,
            }).ToArray();
        }

        private Data.Model.Player TryFindPlayer(ICollection<Data.Model.Player> players, PaymentDataDto payment)
        {
            // First let's try the mail

            var output = players.FirstOrDefault(pl =>
                pl.GoMagicUserId == payment.UserId ||
                pl.EmailAddress.Equals(payment.UserEmail, StringComparison.InvariantCultureIgnoreCase) ||
                pl.EmailAddress == payment.BillingEmail ||
                (payment.BillingName.Contains(pl.FirstName, StringComparison.InvariantCultureIgnoreCase) && payment.BillingName.Contains(pl.LastName, StringComparison.InvariantCultureIgnoreCase)));

            return output;
        }

        protected PaymentDataDto[] ParsePaymentData(DataTable table)
        {
            try
            {
                var paymentData = new List<PaymentDataDto>();

                foreach (DataRow row in table.Rows)
                {
                    var newPaymentData = new PaymentDataDto();

                    var dateString = row.Field<string>("Date");
                    newPaymentData.DateTime = DateTime.Parse(dateString);
                    newPaymentData.BillingEmail = row.Field<string>("Billing Email") ?? "";
                    newPaymentData.USD = double.Parse(row.Field<string>("USD"));
                    newPaymentData.UserEmail = row.Field<string>("User Email") ?? "";
                    newPaymentData.UserId = int.Parse(row.Field<string>("User ID"));
                    newPaymentData.UserName = row.Field<string>("User Name") ?? "";
                    newPaymentData.BillingName = row.Field<string>("Billing Name") ?? "";
                    newPaymentData.Product = row.Field<string>("Product") ?? "";
                    paymentData.Add(newPaymentData);
                }
                return paymentData.ToArray();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task SendNotification()
        {
            var command = new CommandMessage()
            {
                CreatedAtUtc = DateTime.UtcNow,
                Type = "SendUpcomingMatchesNotification",
                Payload = ""
            };

            await _queueDataService.ExecuteAsync(new InsertCommandMessageCommand()
            {
                NewCommand = command,
            });
        }

        public async Task SendRoundStartNotification()
        {
            var command = new CommandMessage()
            {
                CreatedAtUtc = DateTime.UtcNow,
                Type = "SendRoundStartNotification",
                Payload = ""
            };

            await _queueDataService.ExecuteAsync(new InsertCommandMessageCommand()
            {
                NewCommand = command,
            });
        }

        public async Task SyncMatches()
        {
            var command = new CommandMessage()
            {
                CreatedAtUtc = DateTime.UtcNow,
                Type = "SyncMatches",
                Payload = ""
            };

            await _queueDataService.ExecuteAsync(new InsertCommandMessageCommand()
            {
                NewCommand = command,
            });
        }

        public async Task UpdatePlayers()
        {
            try
            {
                await UpdatePlayersList();
            }
            catch (Exception e)
            {
                ;
            }
            try
            {
                await UpdatePlayersPublicProfiles();
            }
            catch (Exception e)
            {
                ;
            }
        }

        public async Task UpdatePlayersList()
        {
            var activeSeason = _leagueDataService.RunQuery(new GetActiveSeasonQuery());

            if (activeSeason == null)
                return;

            var getPlayersRes = await _leagoService.GetPlayers(new Shared.Dto.GetPlayersInDto()
            {
                TournamentKey = activeSeason.LeagoL1Key
            });

            List<Data.Model.Player> toAdd = new List<Data.Model.Player>();

            _leagueDataService.Execute(new UpdatePlayersDataCommand() { Players = getPlayersRes.Players });

            _leagueDataService.Execute(new AddPlayersToSeason() { Players = getPlayersRes.Players, SeasonId = activeSeason.Id });
        }

        public async Task UpdatePlayersPublicProfiles()
        {
            var players = _leagueDataService.RunQuery(new GetPlayersQuery());

            List<PlayerDto> toAdd = new List<PlayerDto>();

            foreach (var player in players)
            {
                var pres = await _leagoService.GetProfile(new GetProfileInDto()
                {
                    ProfileKey = player.LeagoKey,
                    ArenaKey = _leagoOptions.Value.ArenaKey,
                });

                toAdd.Add(new PlayerDto()
                {
                    Id = player.Id,
                    DiscordHandle = pres.DiscordHandle,
                    EmailAddress = pres.Email,
                });
            }

            _leagueDataService.Execute(new UpdatePlayersDataCommand() { Players = toAdd.ToArray() });
        }
    }
}