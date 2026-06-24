using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Models;
using Task = TaskFlow.Domain.Models.Task;

namespace TaskFlow.Infrastructure.Data
{
    public class TaskFlowDbContext : DbContext
    {
        public TaskFlowDbContext(DbContextOptions<TaskFlowDbContext> options)
            : base(options) { }

        public DbSet<Activity> Activities { get; set; }
        public DbSet<Task> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Activity>().HasKey(a => a.Id);
            modelBuilder.Entity<Activity>()
                .HasMany(a => a.Tasks)
                .WithOne(t => t.Activity)
                .HasForeignKey(t => t.ActivityId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Task>().HasKey(t => t.Id);

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
