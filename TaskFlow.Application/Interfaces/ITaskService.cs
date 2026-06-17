using TaskFlow.Domain.Models;
using Task = TaskFlow.Domain.Models.Task;

namespace TaskFlow.Application.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<Task>> GetTasksByActivityAsync(int activityId);
        Task<Task?> GetTaskByIdAsync(int id);
        Task<Task> CreateTaskAsync(Task task);
        Task<Task> UpdateTaskAsync(Task task);
        Task<bool> DeleteTaskAsync(int id);
    }
}
