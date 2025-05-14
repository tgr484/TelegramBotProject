using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Microsoft.EntityFrameworkCore;
using TelegramBotProject.Data;
using TelegramBotProject.Services;
using TelegramBotProject.Bot;
using TelegramBotProject.Bot.CommandHandlers;
using Telegram.Bot.Types;

var botToken = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");
var connString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

if (string.IsNullOrWhiteSpace(botToken) || string.IsNullOrWhiteSpace(connString))
{
    Console.WriteLine("Environment variables TELEGRAM_BOT_TOKEN and DB_CONNECTION_STRING must be set.");
    return;
}

// Setup EF Core
var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlServer(connString);
using var dbContext = new AppDbContext(optionsBuilder.Options);
dbContext.Database.Migrate();

// DI-less simple wiring
var userService = new UserService(dbContext);

// Command registry
var registry = new CommandRegistry();
registry.Register(new StartCommandHandler());
registry.Register(new HelpCommandHandler());
registry.Register(new SetApiKeyCommandHandler());
registry.Register(new GetApiKeyCommandHandler());
registry.Register(new VersionCommandHandler());

// Telegram bot client
var botClient = new TelegramBotClient(botToken);
await botClient.SetMyCommands(new[]
{
    new BotCommand { Command = "start", Description = "Начать работу с ботом" },
    new BotCommand { Command = "help", Description = "Получить справку" },
    new BotCommand { Command = "setapikey", Description = "Назначить апиключ" },
    new BotCommand { Command = "getapikey", Description = "Узнать апиключ" },
    new BotCommand { Command = "version", Description = "Узнать версию приложения" },
});
var botService = new BotService(botClient, registry, userService);

// Start receiving
using var cts = new CancellationTokenSource();

var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
};

botClient.StartReceiving(
    async (client, update, ct) =>
    {
        await botService.HandleUpdateAsync(update);
    },
    async (client, exception, ct) =>
    {
        Console.WriteLine($"Error: {exception}");
    },
    receiverOptions,
    cancellationToken: cts.Token);

var me = await botClient.GetMe();
Console.WriteLine($"🤖 Bot @{me.Username} is up. Press Ctrl+C to exit.");
Console.CancelKeyPress += (s, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

await Task.Delay(-1, cts.Token);
