using KonkursBot.Db.Entities;
using KonkursBot.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KonkursBot.Services
{
    public class GetUserDataServices(IAppDbContext appDbContext)
    {
        private readonly IAppDbContext _context = appDbContext;

        public async Task<List<User>> GetAllUsers() => await _context.Users.ToListAsync();

        public async Task<User?> GetUserById(long Id) => await _context.Users.FirstOrDefaultAsync(x => x.TelegramId == Id);

        public async Task<List<User>> GetAllUserRefferals(long Id) => await _context.Users.Where(x => x.ParentId == Id).ToListAsync();
    }
}
