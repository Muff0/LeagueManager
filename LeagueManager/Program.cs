using LeagueManager.Components;
using LeagueManager.Services;
using Shared.Settings;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Shared.Converter;
using System;
using Microsoft.EntityFrameworkCore;
using Data;
using NetCord.Hosting.Gateway;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.Configure<LeagoSettings>(builder.Configuration.GetSection("Leago"));
builder.Services.Configure<DiscordSettings>(builder.Configuration.GetSection("Discord"));


// Needed to handle bad datetime values without altering the generated code
JsonConvert.DefaultSettings = () => new JsonSerializerSettings
{
    Converters = { new FallbackDateTimeOffsetConverter() },
    NullValueHandling = NullValueHandling.Ignore
};

builder.Services.AddHttpClient();

// Read connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("PostgreSQL");

// Register AppDbContext with PostgreSQL
builder.Services.AddDbContextFactory<LeagueContext>(options =>
    options.UseNpgsql(connectionString));



// Custom services registrations

builder.Services.AddScoped<LeagueDataService>();
builder.Services.AddScoped<LeagoService.LeagoMainService>();
builder.Services.AddScoped<MainService>();

builder.Services.AddScoped<Discord.DiscordService>();

// Start the Discord service 


builder.Services.AddDiscordGateway();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
