using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Models;
using TaskFlow.Infrastructure.Data;
using TaskFlow.Infrastructure.Interfaces;

namespace TaskFlow.Infrastructure.Repositories
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly TaskFlowDbContext _context;

        public ActivityRepository(TaskFlowDbContext context)
        {
            _context = context;
        }

        public async System.Threading.Tasks.Task<IEnumerable<Activity>> GetActivitiesByDateAsync(DateTime date)
        {
            var startDate = date.Date;
            var endDate = startDate.AddDays(1);
            
            return await _context.Activities
                .Where(a => a.Date >= startDate && a.Date < endDate)
                .Include(a => a.Tasks)
                .ToListAsync();
        }

        public async System.Threading.Tasks.Task<Activity?> GetActivityByIdAsync(int id)
        {
            return await _context.Activities
                .Include(a => a.Tasks)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async System.Threading.Tasks.Task<Activity> CreateActivityAsync(Activity activity)
        {
            _context.Activities.Add(activity);
            await SaveChangesAsync();
            return activity;
        }

        public async System.Threading.Tasks.Task<Activity> UpdateActivityAsync(Activity activity)
        {
            _context.Activities.Update(activity);
            await SaveChangesAsync();
            return activity;
        }

        public async System.Threading.Tasks.Task<bool> DeleteActivityAsync(int id)
        {
            var activity = await GetActivityByIdAsync(id);
            if (activity == null) return false;

            _context.Activities.Remove(activity);
            await SaveChangesAsync();
            return true;
        }

        public async System.Threading.Tasks.Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
