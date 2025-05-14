using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotProject.Services;
using TelegramBotProject.Data;

namespace TelegramBotProject.Bot.CommandHandlers
{
    public class SetApiKeyCommandHandler : ICommandHandler
    {
        public string Command => "/setapikey";

        public async Task HandleAsync(ITelegramBotClient botClient, Message message, UserService userService)
        {
            var user = await userService.GetOrCreateUserAsync(message.Chat.Id);
            var parts = message.Text.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2)
            {
                user.ApiKey = parts[1];
                user.State = UserState.None;
                await userService.UpdateUserAsync(user);
                await botClient.SendMessage(message.Chat.Id, "✅ API‑ключ сохранён.");
            }
            else
            {
                await botClient.SendMessage(message.Chat.Id,
                    "❗ Укажите ключ после команды, например: /setapikey ABC123");
            }
        }
    }
}
