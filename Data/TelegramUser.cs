using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TelegramBotProject.Data
{
    public class TelegramUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public long TelegramId { get; set; }
        public string? ApiKey { get; set; }
        public UserState State { get; set; } = UserState.None;
    }

    public enum UserState
    {
        None,
        AwaitingApiKey,
        Default
    }
}
