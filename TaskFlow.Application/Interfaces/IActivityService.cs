using TaskFlow.Domain.Models;

namespace TaskFlow.Application.Interfaces
{
    public interface IActivityService
    {
        Task<IEnumerable<Activity>> GetActivitiesByDateAsync(DateTime date);
        Task<Activity?> GetActivityByIdAsync(int id);
        Task<Activity> CreateActivityAsync(Activity activity);
        Task<Activity> UpdateActivityAsync(Activity activity);
        Task<bool> DeleteActivityAsync(int id);
        Task<IEnumerable<Activity>> GetTodayActivitiesAsync();
    }
}
