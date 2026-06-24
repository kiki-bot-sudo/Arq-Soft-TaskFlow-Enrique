using TaskFlow.Application.Interfaces;
using TaskFlow.Application.Observers;
using TaskFlow.Infrastructure.Interfaces;
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

        public async Task<IEnumerable<Task>> GetTasksByActivityAsync(int activityId)
            => await _repository.GetTasksByActivityAsync(activityId);

        public async Task<Task?> GetTaskByIdAsync(int id)
            => await _repository.GetTaskByIdAsync(id);

        public async Task<Task> CreateTaskAsync(Task task)
        {
            task.CreatedAt = DateTime.UtcNow;
            return await _repository.CreateTaskAsync(task);
        }

        /// <summary>
        /// Actualiza una tarea y notifica a todos los observadores registrados.
        /// </summary>
        public async Task<Task> UpdateTaskAsync(Task task)
        {
            var updated = await _repository.UpdateTaskAsync(task);
            await NotifyObserversAsync(updated);
            return updated;
        }

        public async Task<bool> DeleteTaskAsync(int id)
            => await _repository.DeleteTaskAsync(id);
    }
}
