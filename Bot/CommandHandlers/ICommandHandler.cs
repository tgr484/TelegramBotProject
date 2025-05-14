using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotProject.Services;

namespace TelegramBotProject.Bot.CommandHandlers
{
    public interface ICommandHandler
    {
        string Command { get; }
        Task HandleAsync(ITelegramBotClient botClient, Message message, UserService userService);
    }
}
