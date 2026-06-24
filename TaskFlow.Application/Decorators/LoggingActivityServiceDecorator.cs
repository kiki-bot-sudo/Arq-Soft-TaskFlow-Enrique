using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Models;

namespace TaskFlow.Application.Decorators
{
    /// <summary>
    /// GoF - Patrón Decorator
    /// Envuelve IActivityService agregando logging a cada operación
    /// sin modificar la implementación original (ActivityService).
    /// El servicio decorado no sabe que está siendo observado.
    /// </summary>
    public class LoggingActivityServiceDecorator : IActivityService
    {
        private readonly IActivityService _inner;
        private readonly ILogger<LoggingActivityServiceDecorator> _logger;

        public LoggingActivityServiceDecorator(IActivityService inner,
            ILogger<LoggingActivityServiceDecorator> logger)
        {
            _inner = inner;
            _logger = logger;
        }

        public async Task<IEnumerable<Activity>> GetActivitiesByDateAsync(DateTime date)
        {
            _logger.LogInformation("[ActivityService] GetActivitiesByDate: {Date}", date.ToShortDateString());
            var result = await _inner.GetActivitiesByDateAsync(date);
            _logger.LogInformation("[ActivityService] Returned {Count} activities", result.Count());
            return result;
        }

        public async Task<Activity?> GetActivityByIdAsync(int id)
        {
            _logger.LogInformation("[ActivityService] GetActivityById: {Id}", id);
            return await _inner.GetActivityByIdAsync(id);
        }

        public async Task<Activity> CreateActivityAsync(Activity activity)
        {
            _logger.LogInformation("[ActivityService] CreateActivity: "{Title}"", activity.Title);
            var result = await _inner.CreateActivityAsync(activity);
            _logger.LogInformation("[ActivityService] Created with Id: {Id}", result.Id);
            return result;
        }

        public async Task<Activity> UpdateActivityAsync(Activity activity)
        {
            _logger.LogInformation("[ActivityService] UpdateActivity: Id={Id}", activity.Id);
            return await _inner.UpdateActivityAsync(activity);
        }

        public async Task<bool> DeleteActivityAsync(int id)
        {
            _logger.LogInformation("[ActivityService] DeleteActivity: Id={Id}", id);
            var result = await _inner.DeleteActivityAsync(id);
            _logger.LogInformation("[ActivityService] Delete result: {Result}", result);
            return result;
        }

        public async Task<IEnumerable<Activity>> GetTodayActivitiesAsync()
        {
            _logger.LogInformation("[ActivityService] GetTodayActivities");
            return await _inner.GetTodayActivitiesAsync();
        }
    }
}
