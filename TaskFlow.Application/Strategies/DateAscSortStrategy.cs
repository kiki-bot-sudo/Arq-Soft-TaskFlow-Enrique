using TaskFlow.Domain.Models;

namespace TaskFlow.Application.Strategies
{
    /// <summary>
    /// GoF - Patrón Strategy: Estrategia concreta
    /// Ordena actividades cronológicamente (más próximas primero).
    /// </summary>
    public class DateAscSortStrategy : IActivitySortStrategy
    {
        public IEnumerable<Activity> Sort(IEnumerable<Activity> activities)
        {
            return activities.OrderBy(a => a.Date);
        }
    }
}
