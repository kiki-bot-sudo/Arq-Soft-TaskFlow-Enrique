using TaskFlow.Domain.Models;

namespace TaskFlow.Infrastructure.Interfaces
{
    public interface IActivityRepository
    {
        Task<IEnumerable<Activity>> GetActivitiesByDateAsync(DateTime date);
        Task<Activity?> GetActivityByIdAsync(int id);
        Task<Activity> CreateActivityAsync(Activity activity);
        Task<Activity> UpdateActivityAsync(Activity activity);
        Task<bool> DeleteActivityAsync(int id);
        Task SaveChangesAsync();
    }
}
