using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Models;
using TaskFlow.Infrastructure.Data;
using TaskFlow.Infrastructure.Interfaces;

namespace TaskFlow.Infrastructure.Repositories
{
    /// <summary>
    /// Repositorio para la entidad Activity.
    /// Maneja operaciones CRUD y consultas específicas de actividades.
    /// </summary>
    public class ActivityRepository : BaseRepository<Activity>, IActivityRepository
    {
        public ActivityRepository(TaskFlowDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Obtiene todas las actividades para una fecha específica.
        /// </summary>
        /// <param name="date">Fecha para filtrar (se compara solo la parte de fecha)</param>
        /// <returns>Lista de actividades con sus tareas incluidas</returns>
        public async Task<IEnumerable<Activity>> GetActivitiesByDateAsync(DateTime date)
        {
            var startDate = date.Date;
            var endDate = startDate.AddDays(1);
            
            return await _context.Activities
                .Where(a => a.Date >= startDate && a.Date < endDate)
                .Include(a => a.Tasks)
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene una actividad por ID con sus tareas relacionadas.
        /// </summary>
        /// <param name="id">ID de la actividad</param>
        /// <returns>Actividad con sus tareas, o null si no existe</returns>
        public async Task<Activity?> GetActivityByIdAsync(int id)
        {
            return await _context.Activities
                .Include(a => a.Tasks)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        /// <summary>
        /// Crea una nueva actividad.
        /// </summary>
        /// <param name="activity">Actividad a crear</param>
        /// <returns>Actividad creada</returns>
        public async Task<Activity> CreateActivityAsync(Activity activity)
        {
            await AddAsync(activity);
            await SaveChangesAsync();
            return activity;
        }

        /// <summary>
        /// Actualiza una actividad existente.
        /// </summary>
        /// <param name="activity">Actividad con datos actualizados</param>
        /// <returns>Actividad actualizada</returns>
        public async Task<Activity> UpdateActivityAsync(Activity activity)
        {
            Update(activity);
            await SaveChangesAsync();
            return activity;
        }

        /// <summary>
        /// Elimina una actividad por ID.
        /// </summary>
        /// <param name="id">ID de la actividad a eliminar</param>
        /// <returns>true si se eliminó exitosamente, false si no existe</returns>
        public async Task<bool> DeleteActivityAsync(int id)
        {
            var activity = await GetActivityByIdAsync(id);
            if (activity == null) return false;

            Delete(activity);
            await SaveChangesAsync();
            return true;
        }
    }
}
