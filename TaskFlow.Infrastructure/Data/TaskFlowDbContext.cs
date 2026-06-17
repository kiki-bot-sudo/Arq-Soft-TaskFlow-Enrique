using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Models;
using Task = TaskFlow.Domain.Models.Task;

namespace TaskFlow.Infrastructure.Data
{
    public class TaskFlowDbContext : DbContext
    {
        public TaskFlowDbContext(DbContextOptions<TaskFlowDbContext> options)
            : base(options)
        {
        }

        public DbSet<Activity> Activities { get; set; }
        public DbSet<Task> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Activity configuration
            modelBuilder.Entity<Activity>()
                .HasKey(a => a.Id);
            
            modelBuilder.Entity<Activity>()
                .HasMany(a => a.Tasks)
                .WithOne(t => t.Activity)
                .HasForeignKey(t => t.ActivityId)
                .OnDelete(DeleteBehavior.Cascade);

            // Task configuration
            modelBuilder.Entity<Task>()
                .HasKey(t => t.Id);
        }
    }
}
