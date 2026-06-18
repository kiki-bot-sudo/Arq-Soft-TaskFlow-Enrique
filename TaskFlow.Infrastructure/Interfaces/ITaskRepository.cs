using TaskFlow.Domain.Models;
using Task = TaskFlow.Domain.Models.Task;

namespace TaskFlow.Infrastructure.Interfaces
{
    public interface ITaskRepository
    {
        Task<IEnumerable<Task>> GetTasksByActivityAsync(int activityId);
        Task<Task?> GetTaskByIdAsync(int id);
        Task<Task> CreateTaskAsync(Task task);
        Task<Task> UpdateTaskAsync(Task task);
        Task<bool> DeleteTaskAsync(int id);
        System.Threading.Tasks.Task SaveChangesAsync();
    }
}
