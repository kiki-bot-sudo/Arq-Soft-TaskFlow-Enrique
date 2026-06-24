using TaskFlow.Domain.Models;

namespace TaskFlow.Domain.Builders
{
    /// <summary>
    /// GoF - Patrón Builder
    /// Construye objetos Task paso a paso con valores seguros por defecto.
    /// </summary>
    public class TaskBuilder
    {
        private readonly Task _task = new();

        public TaskBuilder WithActivityId(int activityId)
        {
            _task.ActivityId = activityId;
            return this;
        }

        public TaskBuilder WithTitle(string title)
        {
            _task.Title = title;
            return this;
        }

        public TaskBuilder WithDescription(string description)
        {
            _task.Description = description;
            return this;
        }

        public TaskBuilder WithDueTime(DateTime? dueTime)
        {
            _task.DueTime = dueTime;
            return this;
        }

        /// <summary>
        /// Finaliza la construcción y retorna la Task lista para usar.
        /// CreatedAt se asigna automáticamente.
        /// </summary>
        public Task Build()
        {
            _task.CreatedAt = DateTime.UtcNow;
            return _task;
        }
    }
}
