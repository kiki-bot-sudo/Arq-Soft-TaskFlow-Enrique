namespace TaskFlow.Application.Strategies
{
    /// <summary>
    /// GoF - Patrón Strategy
    /// Define el contrato para los diferentes algoritmos de ordenamiento de actividades.
    /// Permite intercambiar la estrategia de ordenamiento sin modificar el servicio.
    /// </summary>
    public interface IActivitySortStrategy
    {
        IEnumerable<TaskFlow.Domain.Models.Activity> Sort(IEnumerable<TaskFlow.Domain.Models.Activity> activities);
    }
}
