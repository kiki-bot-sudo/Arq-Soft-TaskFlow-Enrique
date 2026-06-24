using TaskFlow.Application.Interfaces;
using TaskFlow.Application.Strategies;
using TaskFlow.Domain.Models;
using TaskFlow.Infrastructure.Interfaces;

namespace TaskFlow.Application.Services
{
    /// <summary>
    /// Servicio de actividades.
    /// Usa el patrón Strategy para el ordenamiento de actividades,
    /// permitiendo cambiar el criterio de orden sin modificar este servicio.
    /// </summary>
    public class ActivityService : IActivityService
    {
        private readonly IActivityRepository _repository;
        private IActivitySortStrategy _sortStrategy;

        public ActivityService(IActivityRepository repository)
        {
            _repository = repository;
            // Estrategia por defecto: ordenar por prioridad descendente
            _sortStrategy = new PriorityDescSortStrategy();
        }

        /// <summary>
        /// Cambia la estrategia de ordenamiento en tiempo de ejecución.
        /// </summary>
        public void SetSortStrategy(IActivitySortStrategy strategy)
        {
            _sortStrategy = strategy;
        }

        public async Task<IEnumerable<Activity>> GetActivitiesByDateAsync(DateTime date)
        {
            var activities = await _repository.GetActivitiesByDateAsync(date);
            return _sortStrategy.Sort(activities);
        }

        public async Task<Activity?> GetActivityByIdAsync(int id)
        {
            return await _repository.GetActivityByIdAsync(id);
        }

        public async Task<Activity> CreateActivityAsync(Activity activity)
        {
            activity.CreatedAt = DateTime.UtcNow;
            return await _repository.CreateActivityAsync(activity);
        }

        public async Task<Activity> UpdateActivityAsync(Activity activity)
        {
            activity.UpdatedAt = DateTime.UtcNow;
            return await _repository.UpdateActivityAsync(activity);
        }

        public async Task<bool> DeleteActivityAsync(int id)
        {
            return await _repository.DeleteActivityAsync(id);
        }

        public async Task<IEnumerable<Activity>> GetTodayActivitiesAsync()
        {
            var today = DateTime.UtcNow.Date;
            return await GetActivitiesByDateAsync(today);
        }
    }
}
