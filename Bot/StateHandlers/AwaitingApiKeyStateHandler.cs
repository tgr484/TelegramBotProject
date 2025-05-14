using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotProject.Services;
using TelegramBotProject.Data;

namespace TelegramBotProject.Bot.StateHandlers
{
    public class AwaitingApiKeyStateHandler
    {
        public async Task HandleAsync(ITelegramBotClient botClient, Message message, TelegramUser user, UserService userService)
        {
            user.ApiKey = message.Text;
            user.State = UserState.Default;
            await userService.UpdateUserAsync(user);

            await botClient.SendMessage(message.Chat.Id, "✅ Ваш API‑ключ сохранён.");
        }
    }
}
