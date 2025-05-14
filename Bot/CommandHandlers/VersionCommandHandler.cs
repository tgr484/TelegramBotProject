using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotProject.Services;
using TelegramBotProject.Data;

namespace TelegramBotProject.Bot.CommandHandlers
{
    public class VersionCommandHandler : ICommandHandler
    {
        public string Command => "/version";

        public async Task HandleAsync(ITelegramBotClient botClient, Message message, UserService userService)
        {
            await botClient.SendMessage(message.Chat.Id,$"v1.02");

        }
    }
}
