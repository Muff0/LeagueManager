using Data;
using Discord;
using LeagoClient;
using LeagoService;
using LeagueCoreService;
using LeagueCoreService.Services;
using Mail;
using Microsoft.EntityFrameworkCore;
using NetCord;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Services.ApplicationCommands;
using Newtonsoft.Json;
using OGS;
using Shared.Converter;
using Shared.Settings;

var builder = Host.CreateApplicationBuilder(args);


builder.Services.Configure<LeagoSettings>(builder.Configuration.GetSection("Leago"));
builder.Services.Configure<DiscordSettings>(builder.Configuration.GetSection("Discord"));
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("Mail"));

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

builder.Services.AddHttpClient<OGSClient>();

// Logging

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

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
builder.Services.AddScoped<LeagoService.LeagoMainService>();
builder.Services.AddScoped<MailService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<OGSService>();
builder.Services.AddScoped<MainService>();
builder.Services.AddScoped<Discord.DiscordService>();
builder.Services.AddScoped<CommandOrchestrator>();

// Start the Discord service

var discordSettings = builder.Configuration
    .GetSection("Discord")
    .Get<DiscordSettings>()!;

builder.Services.AddDiscordGateway(o =>
    {
        o.Token = discordSettings.Token;
    })
    .AddApplicationCommands<ApplicationCommandInteraction, ApplicationCommandContext>()
    .AddGatewayHandlers(typeof(InteractionHandler).Assembly);

builder.Services.AddScoped<LeagueCoreService.Services.MainService>();
builder.Services.AddHostedService<QueueWorker>();
builder.Services.AddHostedService<SchedulerWorker>();

var host = builder.Build();

// Discord command modules

host.AddApplicationCommandModule<BotCommandModule>();

using (var scope = host.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    const int maxRetries = 10;
    var delay = TimeSpan.FromSeconds(3);

    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            var queueDb = services.GetRequiredService<QueueContext>();
            queueDb.Database.Migrate();

            var leagueDb = services.GetRequiredService<LeagueContext>();
            leagueDb.Database.Migrate();

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
}
host.Run();