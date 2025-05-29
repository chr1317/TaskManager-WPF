namespace TaskManager_API
{
    using Microsoft.EntityFrameworkCore;
    using TaskManager_API.Models;

    public class AppDbContext : DbContext
    {
        public DbSet<TaskItem> Tasks { get; set; }

        public DbSet<Users> Users { get; set; }

        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Tasks)
                .HasForeignKey(t => t.CategoryId)
                .HasConstraintName("FK_Tasks_Categories");
            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.User)
                .WithMany(u => u.Tasks)
                .HasForeignKey(t => t.UserId)
                .HasConstraintName("FK_tasks_users_UID");
            
        }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
    }
}
