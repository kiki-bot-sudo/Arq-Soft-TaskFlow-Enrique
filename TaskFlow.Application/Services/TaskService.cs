using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Models;
using Task = TaskFlow.Domain.Models.Task;

namespace TaskFlow.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _repository;

        public TaskService(ITaskRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Task>> GetTasksByActivityAsync(int activityId)
        {
            return await _repository.GetTasksByActivityAsync(activityId);
        }

        public async Task<Task?> GetTaskByIdAsync(int id)
        {
            return await _repository.GetTaskByIdAsync(id);
        }

        public async Task<Task> CreateTaskAsync(Task task)
        {
            task.CreatedAt = DateTime.UtcNow;
            return await _repository.CreateTaskAsync(task);
        }

        public async Task<Task> UpdateTaskAsync(Task task)
        {
            return await _repository.UpdateTaskAsync(task);
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            return await _repository.DeleteTaskAsync(id);
        }
    }
}
