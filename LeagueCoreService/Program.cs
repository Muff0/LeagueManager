using Data;
using Discord;
using Discord.Logging;
using Kifubara;
using Kifubara.Client;
using LeagoClient;
using LeagoService;
using LeagueCoreService;
using LeagueCoreService.Queue;
using LeagueCoreService.ScheduledJobs;
using LeagueCoreService.Services;
using Mail;
using Microsoft.EntityFrameworkCore;
using NetCord;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Services.ApplicationCommands;
using Newtonsoft.Json;
using OGS;
using OGS.Client;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Shared.Converter;
using Shared.Services;
using Shared.Settings;

var builder = Host.CreateApplicationBuilder(args);


builder.Services.Configure<LeagoSettings>(builder.Configuration.GetSection("Leago"));
builder.Services.Configure<DiscordSettings>(builder.Configuration.GetSection("Discord"));
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("Mail"));
builder.Services.Configure<SchedulerSettings>(builder.Configuration.GetSection("Scheduler"));
builder.Services.Configure<OgsSettings>(builder.Configuration.GetSection("Ogs"));
builder.Services.Configure<KifubaraSettings>(builder.Configuration.GetSection("Kifubara"));

// Needed to handle bad datetime values without altering the generated code
JsonConvert.DefaultSettings = () => new JsonSerializerSettings
{
    Converters = { new FallbackDateTimeOffsetConverter() },
    NullValueHandling = NullValueHandling.Ignore
};


// Clients registration

builder.Services.AddHttpClient<AccountClient>();
builder.Services.AddHttpClient<ArenaMembersClient>();
builder.Services.AddHttpClient<ArenasClient>();
builder.Services.AddHttpClient<EventsClient>();
builder.Services.AddHttpClient<HealthClient>();
builder.Services.AddHttpClient<LeaguesClient>();
builder.Services.AddHttpClient<MatchesClient>();
builder.Services.AddHttpClient<PaymentsClient>();
builder.Services.AddHttpClient<ProfilesClient>();
builder.Services.AddHttpClient<RoundsClient>();
builder.Services.AddHttpClient<TournamentsClient>();
builder.Services.AddHttpClient<UsersClient>();

builder.Services.AddHttpClient<IOnlineLeagueClient, OnlineLeagueClient>();

// OGS online_league API: OAuth2 client-credentials auth + bearer token handler.
builder.Services.AddSingleton<IOgsTokenProvider, OgsTokenProvider>();
builder.Services.AddTransient<OgsAuthenticatedHttpHandler>();
builder.Services.AddHttpClient<IGamesClient, GamesClient>(c =>
        c.BaseAddress = new Uri("https://online-go.com/api/v1/"))
    .AddHttpMessageHandler<OgsAuthenticatedHttpHandler>();
builder.Services.AddHttpClient<IOnlineLeagueClient, OnlineLeagueClient>(c =>
        c.BaseAddress = new Uri("https://online-go.com/api/v1/"))
    .AddHttpMessageHandler<OgsAuthenticatedHttpHandler>();
builder.Services.AddHttpClient<IOgsPlayerClient, OgsPlayerClient>(c =>
        c.BaseAddress = new Uri("https://online-go.com/api/v1/"))
    .AddHttpMessageHandler<OgsAuthenticatedHttpHandler>();

// Kifubara API 

builder.Services.AddTransient<KifubaraAuthenticatedHttpHandler>();
var kifubaraSettings = builder.Configuration.GetSection("Kifubara").Get<KifubaraSettings>()!;
builder.Services.AddHttpClient<IKifubaraClient, KifubaraClient>(c =>
    c.BaseAddress = new Uri(kifubaraSettings.BaseUrl))
    .AddHttpMessageHandler<KifubaraAuthenticatedHttpHandler>();

// Logging
var discordSettings = builder.Configuration.GetSection("Discord").Get<DiscordSettings>()!;

var logConfig = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}");

if (!string.IsNullOrEmpty(discordSettings.AlertWebhookUrl))
    logConfig.WriteTo.WriteToDiscordWebhook(
        discordSettings.AlertWebhookUrl,
        LogEventLevel.Error);

Log.Logger = logConfig.CreateLogger();

builder.Services.AddSingleton<ILoggerFactory>(new SerilogLoggerFactory(Log.Logger, true));
builder.Services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

// Read connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("PostgreSQL");

// Register AppDbContext with PostgreSQL
builder.Services.AddDbContextFactory<LeagueContext>(options =>
    options.UseNpgsql(connectionString,
        x =>
        {
            x.MigrationsAssembly("Data");
            x.MigrationsHistoryTable("__LeagueMigrationsHistory");
        }));
// Register AppDbContext with PostgreSQL
builder.Services.AddDbContextFactory<QueueContext>(options =>
    options.UseNpgsql(connectionString,
        x =>
        {
            x.MigrationsAssembly("Data");
            x.MigrationsHistoryTable("__QueueMigrationsHistory");
        }));


// Custom services registrations

builder.Services.AddSingleton<LeagueDataService>();
builder.Services.AddSingleton<QueueDataService>();
builder.Services.AddScoped<LeagoMainService>();
builder.Services.AddScoped<MailService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<OGSService>();
builder.Services.AddScoped<MainService>();
builder.Services.AddScoped<DiscordService>();
builder.Services.AddScoped<CommandOrchestrator>();
builder.Services.AddScoped<KifubaraService>();



// Mail Service

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("Mail"));
builder.Services.AddScoped<MailService>();

// Start the Discord service


builder.Services.AddDiscordGateway(o => { o.Token = discordSettings.Token; })
    .AddApplicationCommands<ApplicationCommandInteraction, ApplicationCommandContext>()
    .AddGatewayHandlers(typeof(InteractionHandler).Assembly);

builder.Services.AddScoped<MainService>();

// Register all the Queue management classes

typeof(SchedulerSettings).Assembly.GetTypes()
    .Where(t => !t.IsAbstract && typeof(IJobSchedulerService).IsAssignableFrom(t))
    .ToList()
    .ForEach(t => builder.Services.AddSingleton(t));

typeof(QueueWorker).Assembly.GetTypes()
    .Where(t => !t.IsAbstract && typeof(ICommandHandler).IsAssignableFrom(t))
    .ToList()
    .ForEach(t => builder.Services.AddScoped(typeof(ICommandHandler), t));

typeof(QueueWorker).Assembly.GetTypes()
    .Where(t => !t.IsAbstract && typeof(IScheduledJob).IsAssignableFrom(t))
    .ToList()
    .ForEach(t => builder.Services.AddSingleton(t));


builder.Services.AddHostedService<QueueWorker>();
builder.Services.AddHostedService<SchedulerWorker>();


var host = builder.Build();

// Discord command modules

host.AddApplicationCommandModule<BotCommandModule>();

using (var scope = host.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    const int maxRetries = 2;
    var delay = TimeSpan.FromSeconds(3);

    for (var i = 0; i < maxRetries; i++)
        try
        {
            var leagueDb = services.GetRequiredService<LeagueContext>();
            leagueDb.Database.Migrate();
            
            var queueDb = services.GetRequiredService<QueueContext>();
            queueDb.Database.Migrate();

            break;
        }
        catch (Exception ex)
        {
            if (i == maxRetries - 1)
                throw;

            Console.WriteLine($"Migration failed. Retrying in {delay.TotalSeconds}s...");
            Thread.Sleep(delay);
        }
}

await host.RunAsync();