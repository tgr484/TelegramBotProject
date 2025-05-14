using System;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBotProject.Bot.FileHandlers
{
    public class PdfFileHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly string _downloadDirectory;

        public PdfFileHandler(ITelegramBotClient botClient, string downloadDirectory = "Downloads")
        {
            _botClient = botClient;
            _downloadDirectory = Path.Combine(AppContext.BaseDirectory, downloadDirectory);
            Directory.CreateDirectory(_downloadDirectory);
        }

        public async Task<bool> TryHandleAsync(Message message)
        {
            if (message.Type != MessageType.Document || message.Document == null)
                return false;

            var document = message.Document;

            if (!IsPdf(document))
            {
                await _botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: $"📄 Файл \"{document.FileName}\" не PDF."
                );
                return false;
            }                

            try
            {
                var file = await _botClient.GetFile(document.FileId);
                var localFilePath = Path.Combine(_downloadDirectory, $"{message.Chat.Id}_{document.FileName}");

                using (var fileStream = new FileStream(localFilePath, FileMode.Create))
                {
                    await _botClient.DownloadFile(file.FilePath, fileStream);
                }

                await _botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: $"📄 Файл \"{document.FileName}\" успешно загружен."
                );

                return true;
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: $"❌ Ошибка при загрузке файла: {ex.Message}"
                );
                return false;
            }
        }

        private bool IsPdf(Document document)
        {
            return document.MimeType == "application/pdf" ||
                   document.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase);
        }
    }
}
