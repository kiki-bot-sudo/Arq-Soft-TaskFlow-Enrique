using TaskFlow.Domain.Models;

namespace TaskFlow.Application.Strategies
{
    /// <summary>
    /// GoF - Patrón Strategy: Estrategia concreta
    /// Ordena actividades de mayor a menor prioridad: High > Normal > Low.
    /// </summary>
    public class PriorityDescSortStrategy : IActivitySortStrategy
    {
        private static readonly Dictionary<string, int> _order = new()
        {
            { "High", 0 },
            { "Normal", 1 },
            { "Low", 2 }
        };

        public IEnumerable<Activity> Sort(IEnumerable<Activity> activities)
        {
            return activities.OrderBy(a => _order.GetValueOrDefault(a.Priority, 1));
        }
    }
}
