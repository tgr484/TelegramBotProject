using TelegramBotProject.Bot.CommandHandlers;

namespace TelegramBotProject.Bot
{
    public class CommandRegistry
    {
        private readonly Dictionary<string, ICommandHandler> _commands = new();

        public void Register(ICommandHandler handler)
        {
            _commands[handler.Command.ToLowerInvariant()] = handler;
        }

        public bool TryGetHandler(string command, out ICommandHandler handler)
        {
            return _commands.TryGetValue(command.ToLowerInvariant(), out handler);
        }
    }
}
