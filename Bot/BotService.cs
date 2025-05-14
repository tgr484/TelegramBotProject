using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using TelegramBotProject.Bot.FileHandlers;
using TelegramBotProject.Bot.StateHandlers;
using TelegramBotProject.Bot;
using TelegramBotProject.Data;
using TelegramBotProject.Services;
using Telegram.Bot.Types;

public class BotService
{
    private readonly ITelegramBotClient _botClient;
    private readonly CommandRegistry _registry;
    private readonly UserService _userService;
    private readonly PdfFileHandler _pdfFileHandler;

    public BotService(ITelegramBotClient botClient, CommandRegistry registry, UserService userService)
    {
        _botClient = botClient;
        _registry = registry;
        _userService = userService;
        _pdfFileHandler = new PdfFileHandler(_botClient);
    }

    public async Task HandleUpdateAsync(Update update)
    {
        if (update.Type != UpdateType.Message || update.Message == null)
            return;

        var message = update.Message;
        var chatId = message.Chat.Id;
        var user = await _userService.GetOrCreateUserAsync(chatId);

        if (await _pdfFileHandler.TryHandleAsync(message))
            return;

        if (message.Type == MessageType.Text && message.Text.StartsWith("/"))
        {
            var cmd = message.Text.Split(' ')[0].ToLowerInvariant();
            if (_registry.TryGetHandler(cmd, out var handler))
            {
                await handler.HandleAsync(_botClient, message, _userService);
            }
            else
            {
                await _botClient.SendMessage(chatId, "❓ Неизвестная команда. Введите /help");
            }
        }
        else if (message.Type == MessageType.Text)
        {
            switch (user.State)
            {
                case UserState.AwaitingApiKey:
                    var apiHandler = new AwaitingApiKeyStateHandler();
                    await apiHandler.HandleAsync(_botClient, message, user, _userService);
                    break;
                default:
                    await _botClient.SendMessage(chatId, "📎 Пожалуйста, введите команду /start.");
                    break;
            }
        }
    }
}
