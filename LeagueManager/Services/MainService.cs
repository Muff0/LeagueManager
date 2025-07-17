using System.Data;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Data;
using Data.Commands.Player;
using Data.Commands.Queue;
using Data.Commands.Review;
using Data.Model;
using Data.Queries;
using Discord;
using LeagoService;
using LeagueManager.Extensions;
using LeagueManager.ViewModel;
using Mail;
using Microsoft.Extensions.Options;
using Shared;
using Shared.Dto;
using Shared.Dto.Discord;
using Shared.Enum;
using Shared.Settings;

namespace LeagueManager.Services
{
    public class MainService : ServiceBase
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

        public async Task LinkReviewMatches()
        {
            try
            {
                await _reviewService.LinkExistingReviewMatches();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        public async Task<bool> UploadOrdersFile(Stream content)
        {
            try
            {
                using var contentStream = new StreamReader(content);
                var table = await BuildOrderDataTable(content);
                var payments = ParsePaymentData(table);
                await ProcessPaymentData(payments);

                return true;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return false;
            }
        }

        public async Task BuildReviews()
        {
            try
            {
                await _reviewService.BuildReviews();
                await _reviewService.AssignRoundsToReviews();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
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

            _leagueDataService.Execute(new UpdatePlayerSeasons() { PlayerRegistrations = toUpdate.ToArray() });
            _leagueDataService.Execute(new UpdatePlayersDataCommand() { Players = updateGoMagicIdList.ToArray() });

            string noRegstr = string.Join(";", noReg.Select(pd => pd.UserEmail));
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

        private Data.Model.Player? TryFindPlayer(ICollection<Data.Model.Player> players, PaymentDataDto payment)
        {
            // First let's try the mail
            Player? res = players.FirstOrDefault(pl => pl.GoMagicUserId == payment.UserId);

            if (res == null)
                res = players.FirstOrDefault(pl => pl.EmailAddress.Equals(payment.UserEmail, StringComparison.InvariantCultureIgnoreCase)
                    || pl.EmailAddress == payment.BillingEmail);

            if (res == null)
            {
                var countSameNames = players.Where(pl => payment.BillingName.Contains(pl.FirstName, StringComparison.InvariantCultureIgnoreCase) && payment.BillingName.Contains(pl.LastName, StringComparison.InvariantCultureIgnoreCase));

                if (countSameNames.Count() == 1)
                    res = countSameNames.First();
            }

            return res;
        }


        public async Task PostReviews(List<ReviewViewModel> reviews)
        {
            try
            {
                var unposted = reviews.Where(re => !string.IsNullOrEmpty(re.ReviewUrl) && re.ReviewStatus != ReviewStatus.Notified).ToList();
                List<ReviewDto> posted = new List<ReviewDto>();
                foreach (var review in unposted)
                {
                    var match = await _leagueDataService.RunQueryAsync(new GetMatchQuery() { Id = (int)review.MatchId!, IncludePlayers = true });
                    if (review.TeacherId == null)
                        continue;
                    var teacher = await _leagueDataService.RunQueryAsync(new GetTeacherQuery() { Id = (int)review.TeacherId! });
                    if (teacher == null)
                        continue;
                    var reviewDto = review.ToReviewDto();
                    await _discordService.PostReviewThread(new PostReviewThreadInDto()
                    {
                        Review = reviewDto,
                        Match = match.ToMatchDto(),
                        Teacher = teacher.ToTeacherDto()
                    });

                    reviewDto.ReviewStatus = ReviewStatus.Notified;
                    posted.Add(reviewDto);
                }

                await _leagueDataService.ExecuteAsync(new UpdateReviewsCommand() { Reviews = posted.ToArray() });
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        protected PaymentDataDto[] ParsePaymentData(DataTable table)
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

        public async Task<List<ReviewViewModel>> GetReviews()
        {
            try
            {
                var season = await _leagueDataService.RunQueryAsync(new GetActiveSeasonQuery());
                var res = await _leagueDataService.RunQueryAsync(new GetReviewsQuery()
                {
                    IncludeMatch = true,
                    IncludeOwner = true,
                    IncludeTeacher = true,
                    Round = [1, 2, 3, 4, 5],
                    SeasonId = season.Id,
                    Status = [ReviewStatus.Planned],
                    MatchQueryMode = GetReviewsQuery.ReviewMatchQueryMode.WithMatchOnly
                });

                return res.Select(re => re.ToViewModel()).ToList();
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return new List<ReviewViewModel> { };
            }
        }

        public async Task SaveReviewChanges(List<ReviewViewModel> reviews)
        {
            var updateList = reviews.Select(re => re.ToReviewDto());

            await _leagueDataService.ExecuteAsync(new UpdateReviewsCommand()
            {
                Reviews = updateList.ToArray(),
            });

        }



        public async Task<List<ReviewViewModel>> GetReviewsToSchedule(int? startIndex, int? count, CancellationToken cancellationToken)
        {

            try
            {
                var season = await _leagueDataService.RunQueryAsync(new GetActiveSeasonQuery());
                var res = await _leagueDataService.RunQueryAsync(new GetReviewsQuery()
                {
                    IncludeMatch = true,
                    IncludeOwner = true,
                    IncludeTeacher = true,
                    Round = [1, 2, 3, 4, 5],
                    SeasonId = season.Id,
                    Status = [ReviewStatus.Planned],
                    MatchQueryMode = GetReviewsQuery.ReviewMatchQueryMode.WithMatchOnly,
                    Count = count ?? 0,
                    StartIndex = startIndex ?? 0,
                });
                return res.Select(rev => rev.ToViewModel()).ToList();
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return new List<ReviewViewModel>();
            }
        }


        public async Task<List<ReviewViewModel>> GetReviewsToRecord(int? startIndex, int? count, CancellationToken cancellationToken)
        {

            try
            {
                var season = await _leagueDataService.RunQueryAsync(new GetActiveSeasonQuery());
                var res = await _leagueDataService.RunQueryAsync(new GetReviewsQuery()
                {
                    IncludeMatch = true,
                    IncludeOwner = true,
                    IncludeTeacher = true,
                    Round = [1, 2, 3, 4, 5],
                    SeasonId = season.Id,
                    Status = [ReviewStatus.Planned],
                    MatchQueryMode = GetReviewsQuery.ReviewMatchQueryMode.WithMatchOnly,
                    Count = count ?? 0,
                    StartIndex = startIndex ?? 0,
                });
                return res.Select(rev => rev.ToViewModel()).ToList();
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return new List<ReviewViewModel>();
            }
        }

        public async Task ScheduleReviews(IEnumerable<ReviewViewModel> reviews, DateTime reviewEventDate, int teacherId)
        {
            try
            {
                var teacher = await _leagueDataService.RunQueryAsync(new GetTeacherQuery() { Id = teacherId });
                if (teacher == null)
                    return;

                var games = new List<MatchDto>();

                foreach (var review in reviews)
                {
                    var toAdd = await _leagueDataService.RunQueryAsync(new GetMatchQuery() { Id = (int)review.MatchId!, IncludePlayers = true });
                    games.Add(toAdd.ToMatchDto());
                }

                await _discordService.SendReviewEventNotification(new SendReviewEventNotificationInDto()
                {
                    DateTimeUTC = reviewEventDate,
                    Reviews = games.ToArray(),
                    Teacher = teacher.ToTeacherDto()
                });

                await _reviewService.SetReviewStatus(new SetReviewStatusInDto()
                {
                    Reviews = reviews.Select(re => re.ToReviewDto()),
                    NewStatus = ReviewStatus.Allocated
                });
            }
            catch (Exception e)
            {
                HandleException(e);
            }
        }

        public async Task SendUpcomingMatchesNotification()
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

        public async Task SendRoundStartNotification(int round, DateTime roundDeadline)
        {
            try
            {
                await _discordService.SendRoundStartNotification(new SendRoundStartNotificationInDto()
                {
                    RoundEnd = roundDeadline,
                    RoundNumber = round
                });
            }
            catch (Exception ex)
            { HandleException(ex); }
        }

        public async Task<List<TeacherViewModel>> GetTeachers()
        {
            try
            {
                var res = await _leagueDataService.RunQueryAsync(new GetTeachersQuery());
                return res.Select(te => te.ToTeacherViewModel()).OrderBy(te => te.Name).ToList();
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return new List<TeacherViewModel>();
            }
        }

        public async Task AssignTeacherToReviews(IEnumerable<ReviewViewModel> reviews, int teacherId) => await _reviewService.AssignTeacherToReviews(reviews.Select(re => re.ToReviewDto()), teacherId);

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

        public async Task SendReviewSchedule(bool sendDiscordMessage = true, bool sendMail = true)
        {
            ReviewScheduleDto[] reviews;
            try
            {
                reviews = await _reviewService.GetReviewSchedule();
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return;
            }
            if (sendDiscordMessage)
            {
                try
                {
                    await _discordService.SendReviewSchedule(reviews);
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
            if (sendMail)
            {
                try
                {

                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
        }

        public async Task UpdatePlayers()
        {
            try
            {
                await UpdatePlayersList();
            }
            catch (Exception e)
            {
                HandleException(e);
            }
            try
            {
                await UpdatePlayersPublicProfiles();
            }
            catch (Exception e)
            {
                HandleException(e);
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