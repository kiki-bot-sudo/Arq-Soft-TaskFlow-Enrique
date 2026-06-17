using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Models;

namespace TaskFlow.Application.Services
{
    public class ActivityService : IActivityService
    {
        private readonly IActivityRepository _repository;

        public ActivityService(IActivityRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Activity>> GetActivitiesByDateAsync(DateTime date)
        {
            var activities = await _repository.GetActivitiesByDateAsync(date);
            return activities.OrderByDescending(a => a.Priority == "High" ? 0 : a.Priority == "Normal" ? 1 : 2);
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
