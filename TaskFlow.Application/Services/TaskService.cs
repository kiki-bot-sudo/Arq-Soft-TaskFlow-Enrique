using TaskFlow.Application.Interfaces;
using TaskFlow.Application.Observers;
using TaskFlow.Infrastructure.Interfaces;
using TaskFlow.Domain.Models;
using Task = TaskFlow.Domain.Models.Task;

namespace TaskFlow.Application.Services
{
    /// <summary>
    /// Servicio de tareas.
    /// Implementa el rol de Sujeto del patrón Observer:
    /// notifica a los observadores registrados cuando una tarea es actualizada.
    /// </summary>
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _repository;
        private readonly List<ITaskObserver> _observers = new();

        public TaskService(ITaskRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Registra un observador que será notificado al actualizar tareas.
        /// </summary>
        public void AddObserver(ITaskObserver observer) => _observers.Add(observer);

        /// <summary>
        /// Elimina un observador previamente registrado.
        /// </summary>
        public void RemoveObserver(ITaskObserver observer) => _observers.Remove(observer);

        private async System.Threading.Tasks.Task NotifyObserversAsync(Task task)
        {
            foreach (var observer in _observers)
                await observer.OnTaskUpdatedAsync(task);
        }

        public async Task<IEnumerable<Task>> GetTasksByActivityAsync(int activityId, string userId)
            => await _repository.GetTasksByActivityAsync(activityId, userId);

        public async Task<IEnumerable<Task>> GetTasksAsync(string userId, string? search, bool? isCompleted,
            string? priority, DateTime? dueDate, string sortBy, bool descending)
            => await _repository.GetTasksAsync(userId, search, isCompleted, priority, dueDate, sortBy, descending);

        public async Task<Task?> GetTaskByIdAsync(int id, string userId)
            => await _repository.GetTaskByIdAsync(id, userId);

        public async Task<Task> CreateTaskAsync(Task task)
        {
            Validate(task);
            if (task.ActivityId <= 0)
                task.ActivityId = await _repository.GetOrCreateDefaultActivityIdAsync();
            task.CreatedAt = DateTime.UtcNow;
            return await _repository.CreateTaskAsync(task);
        }

        /// <summary>
        /// Actualiza una tarea y notifica a todos los observadores registrados.
        /// </summary>
        public async Task<Task> UpdateTaskAsync(Task task)
        {
            Validate(task);
            var updated = await _repository.UpdateTaskAsync(task);
            await NotifyObserversAsync(updated);
            return updated;
        }

        public async Task<bool> DeleteTaskAsync(int id, string userId)
            => await _repository.DeleteTaskAsync(id, userId);

        public async Task<Task?> SetCompletionAsync(int id, bool isCompleted, string userId)
        {
            var task = await _repository.GetTaskByIdAsync(id, userId);
            if (task is null) return null;
            task.IsCompleted = isCompleted;
            return await UpdateTaskAsync(task);
        }

        public async Task<TaskStatistics> GetStatisticsAsync(string userId)
        {
            var tasks = (await _repository.GetTasksAsync(userId, null, null, null, null, "date", false)).ToList();
            var now = DateTime.UtcNow;
            return new TaskStatistics(
                tasks.Count,
                tasks.Count(t => !t.IsCompleted),
                tasks.Count(t => t.IsCompleted),
                tasks.Count(t => !t.IsCompleted && t.DueTime.HasValue && t.DueTime.Value < now));
        }

        public async Task<SubTask?> CreateSubTaskAsync(int taskId, string title, string userId)
        {
            var task = await _repository.GetTaskByIdAsync(taskId, userId);
            if (task is null) return null;
            title = title?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("El título de la subtarea es obligatorio.");
            if (title.Length > 100)
                throw new ArgumentException("El título de la subtarea no puede superar 100 caracteres.");
            return await _repository.CreateSubTaskAsync(new SubTask { TaskId = taskId, Title = title });
        }

        public async Task<SubTask?> SetSubTaskCompletionAsync(
            int taskId, int subTaskId, bool isCompleted, string userId)
        {
            var subTask = await _repository.GetSubTaskAsync(taskId, subTaskId, userId);
            if (subTask is null) return null;
            subTask.IsCompleted = isCompleted;
            await _repository.SaveChangesAsync();
            return subTask;
        }

        public Task<bool> DeleteSubTaskAsync(int taskId, int subTaskId, string userId)
            => _repository.DeleteSubTaskAsync(taskId, subTaskId, userId);

        private static void Validate(Task task)
        {
            task.Title = task.Title?.Trim() ?? string.Empty;
            task.Description = task.Description?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(task.Title))
                throw new ArgumentException("El título es obligatorio.");
            if (task.Title.Length > 100)
                throw new ArgumentException("El título no puede superar 100 caracteres.");
            if (task.Description.Length > 500)
                throw new ArgumentException("La descripción no puede superar 500 caracteres.");
            if (task.Priority is not ("Low" or "Medium" or "High"))
                throw new ArgumentException("La prioridad debe ser Low, Medium o High.");
        }
    }
}
