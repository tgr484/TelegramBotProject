using TelegramBotProject.Data;
using Microsoft.EntityFrameworkCore;

namespace TelegramBotProject.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TelegramUser> GetOrCreateUserAsync(long telegramId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.TelegramId == telegramId);
            if (user == null)
            {
                user = new TelegramUser { TelegramId = telegramId };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            return user;
        }

        public async Task UpdateUserAsync(TelegramUser user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
