using KonkursBot.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace KonkursBot.Interfaces
{
    public interface IAppDbContext
    {
        public DbSet<User> Users { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
