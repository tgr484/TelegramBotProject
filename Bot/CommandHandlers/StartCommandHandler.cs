using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotProject.Services;
using TelegramBotProject.Data;

namespace TelegramBotProject.Bot.CommandHandlers
{
    public class StartCommandHandler : ICommandHandler
    {
        public string Command => "/start";

        public async Task HandleAsync(ITelegramBotClient botClient, Message message, UserService userService)
        {            
            var user = await userService.GetOrCreateUserAsync(message.Chat.Id);
            if (user.State == UserState.None)
            {
                user.State = UserState.AwaitingApiKey;
                await userService.UpdateUserAsync(user);

                await botClient.SendMessage(message.Chat.Id,
                    "👋 Добро пожаловать! Пожалуйста, отправьте ваш API‑ключ:");
            }
            else {
                await botClient.SendMessage(message.Chat.Id,
                    $"Вы уже зарегистрированы, {message.From?.Username}");
            }
            
        }
    }
}
