using Data;
using Discord;
using Havit.Blazor.Components.Web;
using LeagoClient;
using LeagoService;
using LeagueManager.Components;
using LeagueManager.Services;
using Mail;
using Microsoft.EntityFrameworkCore;
using NetCord.Hosting.Gateway;
using Newtonsoft.Json;
using OGS;
using Shared.Converter;
using Shared.Notifications;
using Shared.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHxServices();

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
    options.UseNpgsql(connectionString));
// Register AppDbContext with PostgreSQL
builder.Services.AddDbContextFactory<QueueContext>(options =>
    options.UseNpgsql(connectionString));

// Custom services registrations

builder.Services.AddScoped<LeagueDataService>();
builder.Services.AddScoped<QueueDataService>();
builder.Services.AddScoped<LeagoMainService>();
builder.Services.AddScoped<MailService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<OGSService>();
builder.Services.AddScoped<MainService>();
builder.Services.AddScoped<DiscordService>();
// Notification Services
builder.Services.AddSignalR();
builder.Services.AddSingleton<NotificationDispatcher>();
builder.Services.AddSingleton<INotificationDispatcher>(sp =>
    sp.GetRequiredService<NotificationDispatcher>()); // same instance
builder.Services.AddHostedService<NotificationRelayService>();
builder.Services.AddSingleton<NotificationService>();

// Mail Service

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("Mail"));
builder.Services.AddScoped<MailService>();

// Start the Discord service
builder.Services.AddDiscordGateway();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) app.UseExceptionHandler("/Error", true);

app.UseAntiforgery();


// Map Notification hub

app.MapHub<NotificationHub>("/hubs/notifications");

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();