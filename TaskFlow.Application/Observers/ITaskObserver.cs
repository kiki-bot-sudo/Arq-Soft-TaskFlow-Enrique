using TaskFlow.Domain.Models;
using Task = System.Threading.Tasks.Task;

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
