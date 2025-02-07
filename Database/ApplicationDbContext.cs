
using MassTransit;
using Microsoft.EntityFrameworkCore;
using YprojectUserService.UserFolder.Entities;

namespace YprojectUserService.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserCategory> UserCategories { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            
            modelBuilder.Entity<UserCategory>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.Categories)
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
