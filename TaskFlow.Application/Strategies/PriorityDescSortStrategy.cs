using TaskFlow.Domain.Models;

namespace TaskFlow.Application.Strategies
{
    /// <summary>
    /// GoF - Patrón Strategy: Estrategia concreta
    /// Ordena actividades de mayor a menor prioridad: High > Normal > Low.
    /// </summary>
    public class PriorityDescSortStrategy : IActivitySortStrategy
    {
        public IEnumerable<Activity> Sort(IEnumerable<Activity> activities)
        {
            return activities.OrderBy(a =>
                PriorityLevels.SortOrder.GetValueOrDefault(a.Priority, PriorityLevels.SortOrder[PriorityLevels.Normal]));
        }
    }
}
