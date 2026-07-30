using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using TaskFlow.Domain.Models;
using TaskFlow.Infrastructure.Identity;
using Task = TaskFlow.Domain.Models.Task;

namespace TaskFlow.Infrastructure.Data
{
    public class TaskFlowDbContext : IdentityDbContext<ApplicationUser>
    {
        public TaskFlowDbContext(DbContextOptions<TaskFlowDbContext> options)
            : base(options) { }

        public DbSet<Activity> Activities { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<SubTask> SubTasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Activity>().HasKey(a => a.Id);
            modelBuilder.Entity<Activity>().Property(a => a.Title).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<Activity>().Property(a => a.Description).HasMaxLength(500);
            modelBuilder.Entity<Activity>().Property(a => a.Category).HasMaxLength(50).IsRequired();
            modelBuilder.Entity<Activity>().Property(a => a.Priority).HasMaxLength(10).HasDefaultValue("Normal");
            modelBuilder.Entity<Activity>()
                .HasMany(a => a.Tasks)
                .WithOne(t => t.Activity)
                .HasForeignKey(t => t.ActivityId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Task>().HasKey(t => t.Id);
            modelBuilder.Entity<Task>().Property(t => t.Title).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<Task>().Property(t => t.Description).HasMaxLength(500);
            modelBuilder.Entity<Task>().Property(t => t.Priority).HasMaxLength(10).HasDefaultValue("Medium");
            modelBuilder.Entity<Task>().HasIndex(t => t.DueTime);
            modelBuilder.Entity<Task>().HasIndex(t => new { t.IsCompleted, t.Priority });
            modelBuilder.Entity<Task>().Property(t => t.UserId).HasMaxLength(450);
            modelBuilder.Entity<Task>().HasIndex(t => t.UserId);
            modelBuilder.Entity<Task>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SubTask>().HasKey(s => s.Id);
            modelBuilder.Entity<SubTask>().Property(s => s.Title).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<SubTask>()
                .HasOne(s => s.Task)
                .WithMany(t => t.SubTasks)
                .HasForeignKey(s => s.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed data
            var seedDate = new DateTime(2026, 6, 24, 0, 0, 0, DateTimeKind.Utc);

            modelBuilder.Entity<Activity>().HasData(
                new Activity
                {
                    Id = 1, Title = "Estudiar Arquitectura de Software",
                    Description = "Repasar patrones GoF y ADRs",
                    Date = seedDate, Category = "Estudio",
                    Priority = "High", IsCompleted = false,
                    CreatedAt = seedDate
                },
                new Activity
                {
                    Id = 2, Title = "Entregar proyecto TaskFlow",
                    Description = "Subir avances al repositorio",
                    Date = seedDate, Category = "Universidad",
                    Priority = "High", IsCompleted = false,
                    CreatedAt = seedDate
                },
                new Activity
                {
                    Id = 3, Title = "Hacer ejercicio",
                    Description = "30 min cardio",
                    Date = seedDate, Category = "Salud",
                    Priority = "Normal", IsCompleted = false,
                    CreatedAt = seedDate
                }
            );

            modelBuilder.Entity<Task>().HasData(
                new Task
                {
                    Id = 1, ActivityId = 1,
                    Title = "Leer sobre Strategy Pattern",
                    Description = "Capítulo 5 del libro GoF",
                    IsCompleted = true, CreatedAt = seedDate
                },
                new Task
                {
                    Id = 2, ActivityId = 1,
                    Title = "Implementar Builder en el proyecto",
                    Description = "ActivityBuilder y TaskBuilder",
                    IsCompleted = false, CreatedAt = seedDate
                },
                new Task
                {
                    Id = 3, ActivityId = 2,
                    Title = "Hacer commit de los patrones",
                    Description = "Push a rama api",
                    IsCompleted = false, CreatedAt = seedDate
                }
            );
        }
    }
}
