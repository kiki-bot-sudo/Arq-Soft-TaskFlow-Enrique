using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Models;
using TaskFlow.Infrastructure.Data;
using TaskFlow.Infrastructure.Interfaces;
using Task = TaskFlow.Domain.Models.Task;

namespace TaskFlow.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly TaskFlowDbContext _context;

        public TaskRepository(TaskFlowDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Task>> GetTasksByActivityAsync(int activityId)
        {
            return await _context.Tasks
                .Where(t => t.ActivityId == activityId)
                .ToListAsync();
        }

        public async Task<Task?> GetTaskByIdAsync(int id)
        {
            return await _context.Tasks
                .Include(t => t.Activity)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Task> CreateTaskAsync(Task task)
        {
            _context.Tasks.Add(task);
            await SaveChangesAsync();
            return task;
        }

        public async Task<Task> UpdateTaskAsync(Task task)
        {
            _context.Tasks.Update(task);
            await SaveChangesAsync();
            return task;
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            var task = await GetTaskByIdAsync(id);
            if (task == null) return false;

            _context.Tasks.Remove(task);
            await SaveChangesAsync();
            return true;
        }

        public async System.Threading.Tasks.Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
