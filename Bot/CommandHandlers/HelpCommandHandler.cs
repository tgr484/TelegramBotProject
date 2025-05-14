using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotProject.Services;

namespace TelegramBotProject.Bot.CommandHandlers
{
    public class HelpCommandHandler : ICommandHandler
    {
        public string Command => "/help";

        public async Task HandleAsync(ITelegramBotClient botClient, Message message, UserService userService)
        {
            string help = "📖 Доступные команды:\n" +
                          "/start – начать\n" +
                          "/help – справка\n" +
                          "/setapikey <ключ> – сохранить API‑ключ\n" +
                          "/getapikey - Узнать API‑ключ"; 
            await botClient.SendMessage(message.Chat.Id, help);
        }
    }
}
