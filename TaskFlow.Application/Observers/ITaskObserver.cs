using TaskFlow.Domain.Models;

namespace TaskFlow.Application.Observers
{
    /// <summary>
    /// GoF - Patrón Observer
    /// Contrato que deben implementar todos los observadores de eventos de tarea.
    /// </summary>
    public interface ITaskObserver
    {
        Task OnTaskUpdatedAsync(TaskFlow.Domain.Models.Task task);
    }
}
