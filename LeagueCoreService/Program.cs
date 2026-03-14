using Data;
using LeagoClient;
using LeagoService;
using LeagueCoreService;
using LeagueCoreService.Services;
using Mail;
using Microsoft.EntityFrameworkCore;
using NetCord.Hosting.Gateway;
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

builder.Services.AddSingleton<ITokenProvider, LeagoTokenProvider>();
builder.Services.AddTransient<LeagoAuthenticatedHttpHandler>();

// Clients registration

builder.Services.AddHttpClient<AccountClient>()
    .AddHttpMessageHandler<LeagoAuthenticatedHttpHandler>();
builder.Services.AddHttpClient<ArenaMembersClient>()
    .AddHttpMessageHandler<LeagoAuthenticatedHttpHandler>();
builder.Services.AddHttpClient<ArenasClient>()
    .AddHttpMessageHandler<LeagoAuthenticatedHttpHandler>();
builder.Services.AddHttpClient<EventsClient>()
    .AddHttpMessageHandler<LeagoAuthenticatedHttpHandler>();
builder.Services.AddHttpClient<HealthClient>()
    .AddHttpMessageHandler<LeagoAuthenticatedHttpHandler>();
builder.Services.AddHttpClient<LeaguesClient>()
    .AddHttpMessageHandler<LeagoAuthenticatedHttpHandler>();
builder.Services.AddHttpClient<MatchesClient>()
    .AddHttpMessageHandler<LeagoAuthenticatedHttpHandler>();
builder.Services.AddHttpClient<PaymentsClient>()
    .AddHttpMessageHandler<LeagoAuthenticatedHttpHandler>();
builder.Services.AddHttpClient<ProfilesClient>()
    .AddHttpMessageHandler<LeagoAuthenticatedHttpHandler>();
builder.Services.AddHttpClient<RoundsClient>()
    .AddHttpMessageHandler<LeagoAuthenticatedHttpHandler>();
builder.Services.AddHttpClient<TournamentsClient>()
    .AddHttpMessageHandler<LeagoAuthenticatedHttpHandler>();
builder.Services.AddHttpClient<UsersClient>()
    .AddHttpMessageHandler<LeagoAuthenticatedHttpHandler>();

builder.Services.AddHttpClient<OGSClient>();

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

builder.Services.AddScoped<LeagueDataService>();
builder.Services.AddScoped<QueueDataService>();
builder.Services.AddScoped<LeagoService.LeagoMainService>();
builder.Services.AddScoped<MailService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<OGSService>();
builder.Services.AddScoped<MainService>();
builder.Services.AddScoped<Discord.DiscordService>();

// Start the Discord service
builder.Services.AddDiscordGateway();

builder.Services.AddScoped<LeagueCoreService.Services.MainService>();
builder.Services.AddHostedService<QueueWorker>();
builder.Services.AddHostedService<SchedulerWorker>();

var host = builder.Build();
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