using System.Security.Claims;
using Data;
using Discord;
using Havit.Blazor.Components.Web;
using Kifubara;
using Kifubara.Client;
using LeagoClient;
using LeagoService;
using LeagueManager.Components;
using LeagueManager.Services;
using Mail;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NetCord.Hosting.Gateway;
using Newtonsoft.Json;
using OGS;
using OGS.Client;
using Org.BouncyCastle.Crypto.Generators;
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
builder.Services.Configure<OgsSettings>(builder.Configuration.GetSection("Ogs"));
builder.Services.Configure<KifubaraSettings>(builder.Configuration.GetSection("Kifubara"));

// Needed to handle bad datetime values without altering the generated code
JsonConvert.DefaultSettings = () => new JsonSerializerSettings
{
    Converters = { new FallbackDateTimeOffsetConverter() },
    NullValueHandling = NullValueHandling.Ignore
};

builder.Services.AddSingleton<ITokenProvider, LeagoTokenProvider>();
builder.Services.AddTransient<LeagoAuthenticatedHttpHandler>();


// Clients registration

builder.Services.AddHttpClient<LeagoMainService>();
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

// Kifubara API 

builder.Services.AddTransient<KifubaraAuthenticatedHttpHandler>();
var kifubaraSettings = builder.Configuration.GetSection("Kifubara").Get<KifubaraSettings>()!;
builder.Services.AddHttpClient<IKifubaraClient, KifubaraClient>(c =>
        c.BaseAddress = new Uri(kifubaraSettings.BaseUrl))
    .AddHttpMessageHandler<KifubaraAuthenticatedHttpHandler>();


// OGS online_league API: OAuth2 client-credentials auth + bearer token handler.
builder.Services.AddSingleton<IOgsTokenProvider, OgsTokenProvider>();
builder.Services.AddTransient<OgsAuthenticatedHttpHandler>();
builder.Services.AddHttpClient<IUnauthenticatedOgsClient, UnauthenticatedOgsClient>(c =>
        c.BaseAddress = new Uri("https://online-go.com/api/v1/"));

builder.Services.AddHttpClient<IGamesClient, GamesClient>(c =>
        c.BaseAddress = new Uri("https://online-go.com/api/v1/"))
    .AddHttpMessageHandler<OgsAuthenticatedHttpHandler>();
builder.Services.AddHttpClient<IOnlineLeagueClient, OnlineLeagueClient>(c =>
        c.BaseAddress = new Uri("https://online-go.com/api/v1/"))
    .AddHttpMessageHandler<OgsAuthenticatedHttpHandler>();
builder.Services.AddHttpClient<IOgsPlayerClient, OgsPlayerClient>(c =>
        c.BaseAddress = new Uri("https://online-go.com/api/v1/"))
    .AddHttpMessageHandler<OgsAuthenticatedHttpHandler>();

// Read connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("PostgreSQL");

// Register AppDbContext with PostgreSQL
builder.Services.AddDbContextFactory<LeagueContext>(options =>
    options.UseNpgsql(connectionString));
// Register AppDbContext with PostgreSQL
builder.Services.AddDbContextFactory<QueueContext>(options =>
    options.UseNpgsql(connectionString));

// Mail Service

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("Mail"));
builder.Services.AddScoped<MailService>();

// Notification Services
builder.Services.AddSignalR();
builder.Services.AddSingleton<NotificationDispatcher>();
builder.Services.AddSingleton<INotificationDispatcher>(sp =>
    sp.GetRequiredService<NotificationDispatcher>()); // same instance
builder.Services.AddHostedService<NotificationRelayService>();
builder.Services.AddSingleton<NotificationService>();

// Custom services registrations

builder.Services.AddSingleton<LeagueDataService>();
builder.Services.AddSingleton<QueueDataService>();
builder.Services.AddScoped<LeagoMainService>();
builder.Services.AddScoped<MailService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<OGSService>();
builder.Services.AddScoped<DiscordService>();
builder.Services.AddSingleton<StatService>();
builder.Services.AddScoped<KifubaraService>();
builder.Services.AddScoped<MainService>();
builder.Services.AddScoped<MatchesService>();

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

// Auth components

app.MapPost("/account/login", async (
    [FromForm] string username,
    [FromForm] string password,
    [FromForm] string? returnUrl,
    IOptions<AuthSettings> authSettings,
    HttpContext ctx) =>
{
    var admin = authSettings.Value.Admins
        .FirstOrDefault(a => a.Username == username);

    if (admin is null || !BCrypt.Net.BCrypt.Verify(password, admin.PasswordHash))
    {
        var qs = returnUrl is not null ? $"&returnUrl={Uri.EscapeDataString(returnUrl)}" : "";
        return Results.Redirect($"/login?error=true{qs}");
    }

    var claims = new[] { new Claim(ClaimTypes.Name, admin.Username) };
    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    await ctx.SignInAsync(new ClaimsPrincipal(identity));

    return Results.LocalRedirect(returnUrl ?? "/");
}).DisableAntiforgery();

app.MapGet("/account/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync();
    return Results.Redirect("/login");
}).RequireAuthorization();

app.Run();