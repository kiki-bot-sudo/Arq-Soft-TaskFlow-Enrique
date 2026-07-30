using TaskFlow.Domain.Models;
using Task = TaskFlow.Domain.Models.Task;

namespace TaskFlow.Application.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<Task>> GetTasksByActivityAsync(int activityId, string userId);
        Task<IEnumerable<Task>> GetTasksAsync(string userId, string? search, bool? isCompleted, string? priority,
            DateTime? dueDate, string sortBy, bool descending);
        Task<Task?> GetTaskByIdAsync(int id, string userId);
        Task<Task> CreateTaskAsync(Task task);
        Task<Task> UpdateTaskAsync(Task task);
        Task<bool> DeleteTaskAsync(int id, string userId);
        Task<Task?> SetCompletionAsync(int id, bool isCompleted, string userId);
        Task<TaskStatistics> GetStatisticsAsync(string userId);
        Task<SubTask?> CreateSubTaskAsync(int taskId, string title, string userId);
        Task<SubTask?> SetSubTaskCompletionAsync(int taskId, int subTaskId, bool isCompleted, string userId);
        Task<bool> DeleteSubTaskAsync(int taskId, int subTaskId, string userId);
    }

    public record TaskStatistics(int Total, int Pending, int Completed, int Overdue);
}
