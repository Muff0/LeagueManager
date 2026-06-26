using System.Data;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Data;
using Data.Commands.Player;
using Data.Commands.Queue;
using Data.Commands.Review;
using Data.Extensions;
using Data.Model;
using Data.Queries;
using Discord;
using Kifubara;
using LeagoService;
using LeagueCoreService.Queue;
using LeagueManager.Extensions;
using LeagueManager.ViewModel;
using Mail;
using Mail.MessageBuilders;
using Microsoft.Extensions.Options;
using OGS;
using Shared;
using Shared.Dto;
using Shared.Dto.Discord;
using Shared.Dto.OGS;
using Shared.Enum;
using Shared.Extensions;
using Shared.Notifications;
using Shared.Queue;
using Shared.Settings;

namespace LeagueManager.Services;

public class MainService(QueueDataService queueDataService,
    LeagueDataService leagueDataService,
    LeagoMainService leagoService,
    IOptions<LeagoSettings> leagoOptions,
    DiscordService discordService,
    ReviewService reviewService,
    OGSService ogsService,
    INotificationDispatcher notificationService,
    ILogger<MainService> logger) : ServiceBase(logger)
{

    private readonly Dictionary<string, PlayerParticipationTier> _participationTiers = new()
    {
        { "Go Magic League - Starter", PlayerParticipationTier.DojoTier1 },
        { "Go Magic League - Learner", PlayerParticipationTier.DojoTier2 },
        { "Go Magic League - Adept", PlayerParticipationTier.DojoTier3 },
        { "Go Magic League - Master", PlayerParticipationTier.DojoTier4 },
        //Old product designations, leaving these here just in case
        { "Go Magic League - Participation + Lectures", PlayerParticipationTier.DojoTier1 },
        { "Go Magic League - Lectures + 2 reviews", PlayerParticipationTier.DojoTier2 },
        { "Go Magic League - Lectures + 5 reviews", PlayerParticipationTier.DojoTier3 }
    };



    protected override void HandleException(Exception e)
    {
        base.HandleException(e);
        notificationService.Dispatch(NotificationMessage.Error(e.GetType().ToString(), e.Message, e.Source));
    }


    public async Task<DataTable> BuildOrderDataTable(Stream csvStream)
    {
        var table = new DataTable();

        using var reader = new StreamReader(csvStream);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            DetectDelimiter = true,
            HasHeaderRecord = true
        });

        await csv.ReadAsync();
        csv.ReadHeader();
        foreach (var header in csv.HeaderRecord) table.Columns.Add(header);

        while (await csv.ReadAsync())
        {
            var row = table.NewRow();
            foreach (DataColumn column in table.Columns)
                row[column.ColumnName] = csv.GetField(column.DataType, column.ColumnName) ?? DBNull.Value;
            table.Rows.Add(row);
        }

        return table;
    }

    protected PlayerParticipationTier GetParticipationTier(string product)
    {
        if (!_participationTiers.TryGetValue(product, out var tier))
            return PlayerParticipationTier.None;

        return tier;
    }

    public async Task LinkReviewMatches()
    {
        try
        {
            await reviewService.LinkExistingReviewMatches();
        }
        catch (Exception ex)
        {
            HandleException(ex);
        }
    }

    public async Task<ProcessPaymentDataOutDto> UploadOrdersFile(Stream content)
    {
        try
        {
            using var contentStream = new StreamReader(content);
            var table = await BuildOrderDataTable(content);
            var payments = ParsePaymentData(table);
            var resProcessPayment = await ProcessPaymentData(payments);

            return resProcessPayment;
        }
        catch (Exception ex)
        {
            HandleException(ex);
            return new ProcessPaymentDataOutDto();
        }
    }


    public async Task FetchMissingDiscordIds()
    {
        try
        {
            var players = await leagueDataService.RunQueryAsync(new GetPlayersQuery());

            var missingIds = players.Where(pl => pl.DiscordHandle != null
                                                 && pl.DiscordHandle != ""
                                                 && pl.DiscordId == null);

            var updateList = new List<PlayerDto>();

            foreach (var player in missingIds)
            {
                var id = await discordService.GetDiscordUserId(player.DiscordHandle);
                if (id != null)
                    updateList.Add(new PlayerDto
                    {
                        Id = player.Id,
                        DiscordId = id
                    });
            }

            await leagueDataService.ExecuteAsync(new UpdatePlayersDataCommand { Players = updateList.ToArray() });
            SendTaskCompletedNotification();
        }
        catch (Exception ex)
        {
            HandleException(ex);
        }
    }

    public void SendTaskCompletedNotification()
    {
        notificationService.Dispatch(
            new NotificationMessage(
                "Task Completed",
                null,
                NotificationLevel.Success,
                nameof(MainService),
                DateTime.Now));
    }


    public async Task UpdateDiscordPlayerRole()
    {
        try
        {
            var activeSeason = await leagueDataService.TakeFirstAsync(new GetActiveSeasonQuery());
            var currentPlayers = await leagueDataService.RunQueryAsync(new GetPlayerSeasonsQuery
            {
                IncludePlayer = true,
                SeasonId = activeSeason.Id
            });

            await discordService.UpdatePlayerRole(new UpdatePlayerRoleInDto
            {
                CurrentPlayers = currentPlayers.Select(ps => ps.Player)
                    .Where(pl => pl!.DiscordId != null)
                    .Select(pl => pl!.DiscordId)
                    .Cast<ulong>()
                    .ToArray()
            });
            SendTaskCompletedNotification();
        }
        catch (Exception ex)
        {
            HandleException(ex);
        }
    }

    protected async Task<ProcessPaymentDataOutDto> ProcessPaymentData(PaymentDataDto[] payments)
    {
        var cutoffDate = new DateTime(2026, 4, 1);

        var season = await leagueDataService.TakeFirstAsync(new GetActiveSeasonQuery { IncludePlayerSeasons = true });
        var players = await leagueDataService.RunQueryAsync(new GetPlayersQuery());

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
                    new PlayerDto
                    {
                        Id = player.Id,
                        GoMagicUserId = payment.UserId
                    });

            var currentPlayerSeason = season.PlayerSeasons.FirstOrDefault(ps => ps.PlayerId == player.Id);

            if (currentPlayerSeason == null)
            {
                noReg.Add(payment);
                continue;
            }

            var partTier = GetParticipationTier(payment.Product);

            if (currentPlayerSeason.PaymentStatus == PlayerPaymentStatus.Refunded ||
                currentPlayerSeason.PaymentStatus == PlayerPaymentStatus.Cancelled)
                continue;

            if (currentPlayerSeason.PaymentStatus == PlayerPaymentStatus.None
                || currentPlayerSeason.ParticipationTier != partTier)
                toUpdate.Add(new PlayerRegistrationDto
                {
                    DateTime = payment.DateTime,
                    PlayerId = player.Id,
                    PlayerParticipationTier = partTier,
                    SeasonId = season.Id,
                    PlayerPaymentStatus = PlayerPaymentStatus.Paid
                });
        }

        leagueDataService.Execute(new UpdatePlayerSeasons { PlayerRegistrations = toUpdate.ToArray() });
        leagueDataService.Execute(new UpdatePlayersDataCommand { Players = updateGoMagicIdList.ToArray() });

        return new ProcessPaymentDataOutDto
        {
            NoMatches = noMatch.ToArray(),
            MissingRegistrations = noReg.ToArray()
        };
    }

    public async Task<PlayerDto[]> GetMissingPayments()
    {
        var season = await leagueDataService.TakeFirstAsync(new GetActiveSeasonQuery());
        var resGetMp = await leagueDataService.RunQueryAsync(new GetMissingPaymentsQuery { SeasonId = season.Id });

        return resGetMp.Select(pl => new PlayerDto
        {
            FirstName = pl.FirstName,
            LastName = pl.LastName,
            EmailAddress = pl.EmailAddress,
            DiscordHandle = pl.DiscordHandle
        }).ToArray();
    }

    private Data.Model.Player? TryFindPlayer(ICollection<Data.Model.Player> players, PaymentDataDto payment)
    {
        // First let's try the mail
        var res = players.FirstOrDefault(pl => pl.GoMagicUserId == payment.UserId);

        if (res == null)
            res = players.FirstOrDefault(pl =>
                pl.EmailAddress.Equals(payment.UserEmail, StringComparison.InvariantCultureIgnoreCase)
                || pl.EmailAddress == payment.BillingEmail);

        if (res == null)
        {
            var countSameNames = players.Where(pl =>
                payment.BillingName.Contains(pl.FirstName, StringComparison.InvariantCultureIgnoreCase) &&
                payment.BillingName.Contains(pl.LastName, StringComparison.InvariantCultureIgnoreCase));

            if (countSameNames.Count() == 1)
                res = countSameNames.First();
        }

        return res;
    }

    public async Task<CheckRankDto[]> CheckRanks()
    {
        try
        {
            var results = new List<CheckRankDto>();
            var season = await leagueDataService.TakeFirstAsync(new GetActiveSeasonQuery());
            var players = await leagueDataService.RunQueryAsync(new GetPlayerSeasonsQuery
            {
                IncludePlayer = true,
                SeasonId = season.Id
            });

            foreach (var player in players)
            {
                if (player.Player == null || player.Player.OGSHandle == "")
                    continue;

                var ogsPl = await ogsService.GetPlayer(player.Player.OGSHandle);

                if (ogsPl == null)
                    continue;

                results.Add(new CheckRankDto
                {
                    Player = player.Player.ToPlayerDto(),
                    OGSRank = ogsService.RatingToRank(ogsPl.Rating),
                    OGSRating = ogsPl.Rating
                });
            }

            return results.ToArray();
        }
        catch (Exception ex)
        {
            HandleException(ex);
            return [];
        }
    }

    public async Task PostReviews()
    {
        try
        {
            var unposted = await leagueDataService.RunQueryAsync(new GetReviewsQuery
            {
                IncludeMatch = true,
                IncludeTeacher = true,
                HasReviewUrl = true,
                IncludePlayerSeasons = true,
                Status = [ReviewStatus.Allocated]
            });

            var posted = new List<ReviewDto>();
            foreach (var review in unposted)
            {
                var reviewDto = review.ToReviewDto();
                await discordService.PostReviewThread(new PostReviewThreadInDto
                {
                    Review = reviewDto,
                    Match = review.Match!.ToMatchDto(),
                    Teacher = review.Teacher!.ToTeacherDto()
                });

                reviewDto.ReviewStatus = ReviewStatus.Notified;
                posted.Add(reviewDto);
            }

            await leagueDataService.ExecuteAsync(new UpdateReviewsCommand { Reviews = posted.ToArray() });
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


    public async Task SaveReviewChanges(List<ReviewViewModel> reviews)
    {
        var updateList = reviews.Select(re => re.ToReviewDto());

        await leagueDataService.ExecuteAsync(new UpdateReviewsCommand
        {
            Reviews = updateList.ToArray()
        });
    }

    public async Task AddReviewToPlayer(PlayerViewModel player)
    {
        await reviewService.AddReviewToPlayer(player.ToPlayerDto());
    }

    public async Task SavePlayerChanges(List<PlayerViewModel> players)
    {
        var updateList = players.Select(pl => pl.ToPlayerDto());

        await leagueDataService.ExecuteAsync(new UpdatePlayersDataCommand
        {
            Players = updateList.ToArray()
        });
    }


    public async Task<List<PlayerViewModel>> GetPlayers(int? startIndex, int? count,
        CancellationToken cancellationToken)
    {
        try
        {
            var season = await leagueDataService.TakeFirstAsync(new GetActiveSeasonQuery());
            var res = await leagueDataService.RunQueryAsync(new GetPlayersQuery
            {
                IncludePlayerSeasons = true,
                SeasonId = season.Id,
                Count = count ?? 0,
                StartIndex = startIndex ?? 0,
                OrderResults = true
            });
            return res.Select(rev => rev.ToPlayerViewModel()).ToList();
        }
        catch (Exception ex)
        {
            HandleException(ex);
            return new List<PlayerViewModel>();
        }
    }

    public async Task<List<RankChangeRequestViewModel>> GetRankChangeRequests(int? startIndex, int? count,
        CancellationToken cancellationToken)
    {
        try
        {
            var res = await queueDataService.RunQueryAsync(new GetCommandMessagesQuery
            {
                Types = ["RankChangeCommand"]
            });
            return res.Select(rev => rev.ToRankChangeRequestViewModel()).ToList();
        }
        catch (Exception ex)
        {
            HandleException(ex);
            return new List<RankChangeRequestViewModel>();
        }
    }


    public async Task MarkRankChangeRequestComplete(RankChangeRequestViewModel request)
    {
        try
        {
            queueDataService.Execute(new SetCommandMessageStatusCommand
            {
                CommandMessageId = request.Id,
                NewStatus = QueueStatus.Completed
            });
        }
        catch (Exception e)
        {
            HandleException(e);
            ;
        }
    }

    public async Task<List<ReviewViewModel>> GetMissedReviews(int? startIndex, int? count,
        CancellationToken cancellationToken)
    {
        try
        {
            var season = await leagueDataService.TakeFirstAsync(new GetActiveSeasonQuery());
            var res = await leagueDataService.RunQueryAsync(new GetReviewsQuery
            {
                IncludeMatch = true,
                IncludeOwner = true,
                IncludeTeacher = true,
                Round = [1, 2, 3, 4, 5],
                ExcludeSeasonIds = [season.Id],
                Status = [ReviewStatus.Planned, ReviewStatus.Allocated],
                Count = count ?? 0,
                StartIndex = startIndex ?? 0
            });
            return res.Select(rev => rev.ToViewModel()).ToList();
        }
        catch (Exception ex)
        {
            HandleException(ex);
            return new List<ReviewViewModel>();
        }
    }


    public async Task DeleteReviews(IEnumerable<ReviewViewModel> reviews)
    {
        try
        {
            await leagueDataService.ExecuteAsync(new DeleteReviewsCommand
                { Reviews = reviews.Select(re => re.ToReviewDto()).ToArray() });
        }
        catch (Exception e)
        {
            HandleException(e);
        }
    }

    public async Task UnlinkMatchFromReviews(IEnumerable<ReviewViewModel> reviews, bool deleteMatch = false)
    {
        try
        {
            await reviewService.UnlinkMatch(reviews.Select(re => re.ToReviewDto()).ToArray(), deleteMatch);
        }
        catch (Exception e)
        {
            HandleException(e);
        }
    }


    public async Task SetCanceledReview(IEnumerable<ReviewViewModel> reviews)
    {
        try
        {
            await reviewService.SetReviewStatus(new SetReviewStatusInDto
            {
                Reviews = reviews.Select(re => re.ToReviewDto()),
                NewStatus = ReviewStatus.Canceled
            });
        }
        catch (Exception e)
        {
            HandleException(e);
        }
    }

    public async Task ScheduleReviews(IEnumerable<ReviewViewModel> reviews, DateTime reviewEventDate, int teacherId)
    {
        try
        {
            var teacher = await leagueDataService.TakeFirstAsync(new GetTeacherQuery { Id = teacherId });
            if (teacher == null)
                return;

            var games = new List<MatchDto>();
            var toAssign = new List<ReviewDto>();

            foreach (var review in reviews)
            {
                var toAdd = await leagueDataService.TakeFirstAsync(new GetMatchQuery
                    { Id = (int)review.MatchId!, IncludePlayers = true });
                if (toAdd == null)
                    continue;
                games.Add(toAdd.ToMatchDto());
                toAssign.Add(review.ToReviewDto());
            }

            await reviewService.AssignTeacherToReviews(
                toAssign.ToArray(),
                teacher.Id);
            
            await reviewService.MoveToAllocated(toAssign);

            await discordService.SendReviewEventNotification(new SendReviewEventNotificationInDto
            {
                DateTimeUTC = reviewEventDate,
                Reviews = games.ToArray(),
                Teacher = teacher.ToTeacherDto()
            });
        }
        catch (Exception e)
        {
            HandleException(e);
        }
    }

    public async Task SendUpcomingMatchesNotification()
    {
        var command = new CommandMessage
        {
            CreatedAtUtc = DateTime.UtcNow,
            Type = "SendUpcomingMatchesNotification",
            Payload = ""
        };

        await queueDataService.ExecuteAsync(new InsertCommandMessageCommand
        {
            NewCommand = command
        });
    }

    public async Task SendRoundStartNotification(int round, DateTime roundDeadline)
    {
        try
        {
            await discordService.SendRoundStartNotification(new SendRoundStartNotificationInDto
            {
                RoundEnd = roundDeadline,
                RoundNumber = round
            });
            var season = await leagueDataService.TakeFirstAsync(new GetActiveSeasonQuery());
            var players = await leagueDataService.RunQueryAsync(new GetPlayersForRoundQuery
            {
                SeasonId = season.Id,
                Round = round
            });

            var message = new RoundStartMessage(season.ToSeasonDto(), round, roundDeadline);

            queueDataService.ExecuteAsync(new InsertCommandMessageCommand
            {
                NewCommand = new CommandMessage
                {
                    Type = "SendEmail",
                    Payload = new SendEmailPayload
                    {
                        Bccs = players.Select(pl => pl.EmailAddress).ToArray(),
                        Subject = message.Subject,
                        HtmlBody = message.HtmlBody
                    }.SerializePayload()
                }
            });
        }
        catch (Exception ex)
        {
            HandleException(ex);
        }
    }

    public async Task<List<TeacherViewModel>> GetTeachers()
    {
        try
        {
            var res = await leagueDataService.RunQueryAsync(new GetTeachersQuery());
            return res.Select(te => te.ToTeacherViewModel()).OrderBy(te => te.Name).ToList();
        }
        catch (Exception ex)
        {
            HandleException(ex);
            return new List<TeacherViewModel>();
        }
    }

    public async Task AssignTeacherToReviews(IEnumerable<ReviewViewModel> reviews, int teacherId)
    {
        await reviewService.AssignTeacherToReviews(reviews.Select(re => re.ToReviewDto()), teacherId);
    }

    public async Task SyncMatches()
    {
        var command = new CommandMessage
        {
            CreatedAtUtc = DateTime.UtcNow,
            Type = "SyncMatches",
            Payload = ""
        };

        await queueDataService.ExecuteAsync(new InsertCommandMessageCommand
        {
            NewCommand = command
        });
    }

    public async Task SendReviewSchedule(bool sendDiscordMessage = true, bool sendMail = true)
    {
        ReviewScheduleDto[] reviews;
        try
        {
            reviews = await reviewService.GetReviewSchedule();
        }
        catch (Exception ex)
        {
            HandleException(ex);
            return;
        }

        if (sendDiscordMessage)
            try
            {
                await discordService.SendReviewSchedule(reviews);
            }
            catch (Exception ex)
            {
                HandleException(ex);
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

        SendTaskCompletedNotification();
    }

    public async Task UpdatePlayersList()
    {
        var activeSeason = await leagueDataService.TakeFirstAsync(new GetActiveSeasonQuery());

        if (activeSeason == null)
            return;

        var getPlayersRes = await leagoService.GetPlayers(new GetPlayersInDto
        {
            TournamentKey = activeSeason.LeagoL1Key
        });

        var toAdd = new List<Data.Model.Player>();

        leagueDataService.Execute(new UpdatePlayersDataCommand { Players = getPlayersRes.Players });

        leagueDataService.Execute(new AddPlayersToSeason
            { Players = getPlayersRes.Players, SeasonId = activeSeason.Id });
    }

    public async Task UpdatePlayersPublicProfiles()
    {
        var players = leagueDataService.RunQuery(new GetPlayersQuery());

        var toAdd = new List<PlayerDto>();

        foreach (var player in players)
        {
            var pres = await leagoService.GetProfile(new GetProfileInDto
            {
                ProfileKey = player.LeagoKey,
                ArenaKey = leagoOptions.Value.ArenaKey
            });

            toAdd.Add(new PlayerDto
            {
                Id = player.Id,
                DiscordHandle = pres.DiscordHandle,
                EmailAddress = pres.Email,
                TimeZone = pres.Timezone
            });
        }

        leagueDataService.Execute(new UpdatePlayersDataCommand { Players = toAdd.ToArray() });
    }

    public async Task SendMissingPaymentsEmail()
    {
        try
        {
            SendTaskCompletedNotification();
        }
        catch (Exception e)
        {
            HandleException(e);
        }
    }

    public async Task<PlayerSeasonViewModel[]> GetPlayerSeasons(int playerId)
    {
        var res = await leagueDataService.RunQueryAsync(
            new GetPlayerSeasonsQuery()
            {
                IncludeSeason = true,
                PlayerId = playerId
            });

        return res.Select(ps => new PlayerSeasonViewModel()
        {
            PlayerId = ps.PlayerId,
            SeasonId = ps.SeasonId,
            SeasonTitle = ps.Season?.Title ?? "",
            PlayerParticipationTier = ps.ParticipationTier
        }).ToArray();
    }
}