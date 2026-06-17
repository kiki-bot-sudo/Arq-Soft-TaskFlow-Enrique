using TaskFlow.Domain.Models;

namespace TaskFlow.Infrastructure.Interfaces
{
    public interface ITaskRepository
    {
        Task<IEnumerable<Task>> GetTasksByActivityAsync(int activityId);
        Task<Task?> GetTaskByIdAsync(int id);
        Task<Task> CreateTaskAsync(Task task);
        Task<Task> UpdateTaskAsync(Task task);
        Task<bool> DeleteTaskAsync(int id);
        Task SaveChangesAsync();
    }
}
