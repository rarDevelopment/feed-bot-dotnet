global using Discord;
global using Discord.Interactions;
global using Discord.WebSocket;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.Logging;
using DiscordDotNetUtilities;
using DiscordDotNetUtilities.Interfaces;
using FeedBot;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Reflection;
using FeedBot.DataLayer;
using FeedBot.Models;
using NodaTime;

var builder = new HostBuilder();

builder.ConfigureAppConfiguration(options
    => options.AddJsonFile("appsettings.json")
        .AddUserSecrets(Assembly.GetEntryAssembly(), true)
        .AddEnvironmentVariables())
    .ConfigureHostConfiguration(configHost =>
    {
        configHost.AddEnvironmentVariables(prefix: "DOTNET_");
    });

var loggerConfig = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File($"logs/log-{DateTime.Now:dd.MM.yy_HH.mm}.log")
    .CreateLogger();

builder.ConfigureServices((host, services) =>
{
    services.AddLogging(options => options.AddSerilog(loggerConfig, dispose: true));
    services.AddSingleton(new DiscordSocketClient(
        new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.AllUnprivileged,
            FormatUsersInBidirectionalUnicode = false,
            LogGatewayIntentWarnings = false
        }));

    var discordSettings = new DiscordSettings(host.Configuration["Discord:BotToken"]!);
    var versionSettings = new VersionSettings(host.Configuration["Version:VersionNumber"]!);
    var databaseSettings = new DatabaseSettings
    {
        Cluster = host.Configuration["Database:Cluster"],
        User = host.Configuration["Database:User"],
        Password = host.Configuration["Database:Password"],
        Name = host.Configuration["Database:Name"],
    };

    services.AddSingleton(discordSettings);
    services.AddSingleton(databaseSettings);
    services.AddSingleton(versionSettings);
    services.AddScoped<IDiscordFormatter, DiscordFormatter>();
    services.AddScoped<IFeedDataLayer, FeedDataLayer>();
    services.AddScoped<IFeedProcessor, FeedProcessor>();

    IClock clock = SystemClock.Instance;
    services.AddTransient(_ => clock);

    services.AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()));

    services.AddSingleton<InteractionHandler>();

    services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(DiscordBot).GetTypeInfo().Assembly));

    services.AddHostedService<DiscordBot>();
});

var app = builder.Build();

await app.RunAsync();
