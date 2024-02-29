using KonkursBot.Db.Entities;
using KonkursBot.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KonkursBot.Db
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasKey(x => x.TelegramId);
            modelBuilder.Entity<User>().HasIndex(x => x.PhoneNumber).IsUnique(true);
        }
    }
}
