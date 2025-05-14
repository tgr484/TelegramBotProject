using Microsoft.EntityFrameworkCore;

namespace TelegramBotProject.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<TelegramUser> Users => Set<TelegramUser>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
    }
}
