using TaskFlow.Domain.Models;
using Task = TaskFlow.Domain.Models.Task;

namespace TaskFlow.Infrastructure.Interfaces
{
    public interface ITaskRepository
    {
        Task<IEnumerable<Task>> GetTasksByActivityAsync(int activityId, string userId);
        Task<IEnumerable<Task>> GetTasksAsync(string userId, string? search, bool? isCompleted, string? priority,
            DateTime? dueDate, string sortBy, bool descending);
        Task<Task?> GetTaskByIdAsync(int id, string userId);
        Task<Task> CreateTaskAsync(Task task);
        Task<Task> UpdateTaskAsync(Task task);
        Task<bool> DeleteTaskAsync(int id, string userId);
        System.Threading.Tasks.Task SaveChangesAsync();
        Task<int> GetOrCreateDefaultActivityIdAsync();
        Task<SubTask> CreateSubTaskAsync(SubTask subTask);
        Task<SubTask?> GetSubTaskAsync(int taskId, int subTaskId, string userId);
        Task<bool> DeleteSubTaskAsync(int taskId, int subTaskId, string userId);
    }
}
