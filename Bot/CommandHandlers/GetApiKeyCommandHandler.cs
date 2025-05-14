using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotProject.Services;
using TelegramBotProject.Data;

namespace TelegramBotProject.Bot.CommandHandlers
{
    public class GetApiKeyCommandHandler : ICommandHandler
    {
        public string Command => "/getapikey";

        public async Task HandleAsync(ITelegramBotClient botClient, Message message, UserService userService)
        {
            var user = await userService.GetOrCreateUserAsync(message.Chat.Id);
            if(user.State == UserState.Default)
            {
                await botClient.SendMessage(message.Chat.Id, $"Ваш API-ключ: {user.ApiKey}");
            }
            else
            {
                await botClient.SendMessage(message.Chat.Id,
                    "❗ Укажите ключ после команды, например: /setapikey ABC123");
            }
        }
    }
}
