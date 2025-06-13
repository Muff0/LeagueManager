using Data;
using LeagoService;
using LeagueCoreService;
using LeagueCoreService.Services;
using Microsoft.EntityFrameworkCore;
using NetCord.Hosting.Gateway;
using Newtonsoft.Json;
using Shared.Converter;
using Shared.Settings;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<LeagoSettings>(builder.Configuration.GetSection("Leago"));
builder.Services.Configure<DiscordSettings>(builder.Configuration.GetSection("Discord"));


// Needed to handle bad datetime values without altering the generated code
JsonConvert.DefaultSettings = () => new JsonSerializerSettings
{
    Converters = { new FallbackDateTimeOffsetConverter() },
    NullValueHandling = NullValueHandling.Ignore
};



builder.Services.AddHttpClient<LeagoTokenProvider>();
builder.Services.AddTransient<LeagoAuthenticatedHttpHandler>();
builder.Services.AddHttpClient<LeagoMainService>()
    .AddHttpMessageHandler<LeagoAuthenticatedHttpHandler>();

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
builder.Services.AddScoped<LeagoService.LeagoMainService>();
builder.Services.AddScoped<MainService>();
builder.Services.AddScoped<Discord.DiscordService>();
builder.Services.AddHostedService<Worker>();


// Start the Discord service 


builder.Services.AddDiscordGateway();




var host = builder.Build();
host.Run();
