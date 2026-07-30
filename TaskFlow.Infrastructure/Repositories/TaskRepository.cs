using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Models;
using TaskFlow.Infrastructure.Data;
using TaskFlow.Infrastructure.Interfaces;
using Task = TaskFlow.Domain.Models.Task;

namespace TaskFlow.Infrastructure.Repositories
{
    /// <summary>
    /// Repositorio para la entidad Task.
    /// Maneja operaciones CRUD y consultas específicas de tareas.
    /// </summary>
    public class TaskRepository : BaseRepository<Task>, ITaskRepository
    {
        public TaskRepository(TaskFlowDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Obtiene todas las tareas asociadas a una actividad específica.
        /// </summary>
        /// <param name="activityId">ID de la actividad</param>
        /// <returns>Lista de tareas</returns>
        public async Task<IEnumerable<Task>> GetTasksByActivityAsync(int activityId, string userId)
        {
            return await _context.Tasks
                .AsNoTracking()
                .Include(t => t.SubTasks)
                .Where(t => t.ActivityId == activityId && t.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Task>> GetTasksAsync(string userId, string? search, bool? isCompleted,
            string? priority, DateTime? dueDate, string sortBy, bool descending)
        {
            var query = _context.Tasks.AsNoTracking()
                .Include(t => t.SubTasks)
                .Where(t => t.UserId == userId);
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(t => t.Title.Contains(term));
            }
            if (isCompleted.HasValue)
                query = query.Where(t => t.IsCompleted == isCompleted.Value);
            if (!string.IsNullOrWhiteSpace(priority))
                query = query.Where(t => t.Priority == priority);
            if (dueDate.HasValue)
            {
                var start = dueDate.Value.Date;
                var end = start.AddDays(1);
                query = query.Where(t => t.DueTime >= start && t.DueTime < end);
            }

            query = sortBy.ToLowerInvariant() switch
            {
                "name" => descending ? query.OrderByDescending(t => t.Title) : query.OrderBy(t => t.Title),
                "priority" => descending
                    ? query.OrderByDescending(t => t.Priority == "High" ? 0 : t.Priority == "Medium" ? 1 : 2)
                    : query.OrderBy(t => t.Priority == "High" ? 0 : t.Priority == "Medium" ? 1 : 2),
                _ => descending ? query.OrderByDescending(t => t.DueTime) : query.OrderBy(t => t.DueTime)
            };
            return await query.ToListAsync();
        }

        /// <summary>
        /// Obtiene una tarea por ID con su actividad relacionada.
        /// </summary>
        /// <param name="id">ID de la tarea</param>
        /// <returns>Tarea con su actividad, o null si no existe</returns>
        public async Task<Task?> GetTaskByIdAsync(int id, string userId)
        {
            return await _context.Tasks
                .Include(t => t.Activity)
                .Include(t => t.SubTasks)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        }

        /// <summary>
        /// Crea una nueva tarea.
        /// </summary>
        /// <param name="task">Tarea a crear</param>
        /// <returns>Tarea creada</returns>
        public async Task<Task> CreateTaskAsync(Task task)
        {
            await AddAsync(task);
            await SaveChangesAsync();
            return task;
        }

        /// <summary>
        /// Actualiza una tarea existente.
        /// </summary>
        /// <param name="task">Tarea con datos actualizados</param>
        /// <returns>Tarea actualizada</returns>
        public async Task<Task> UpdateTaskAsync(Task task)
        {
            Update(task);
            await SaveChangesAsync();
            return task;
        }

        /// <summary>
        /// Elimina una tarea por ID.
        /// </summary>
        /// <param name="id">ID de la tarea a eliminar</param>
        /// <returns>true si se eliminó exitosamente, false si no existe</returns>
        public async Task<bool> DeleteTaskAsync(int id, string userId)
        {
            var task = await GetTaskByIdAsync(id, userId);
            if (task == null) return false;

            Delete(task);
            await SaveChangesAsync();
            return true;
        }

        public async Task<int> GetOrCreateDefaultActivityIdAsync()
        {
            const string title = "Agenda personal";
            var activity = await _context.Activities.FirstOrDefaultAsync(a => a.Title == title);
            if (activity is not null) return activity.Id;

            activity = new Activity
            {
                Title = title,
                Description = "Contenedor predeterminado para las tareas personales.",
                Date = DateTime.UtcNow.Date,
                Category = "General",
                Priority = "Normal",
                CreatedAt = DateTime.UtcNow
            };
            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();
            return activity.Id;
        }

        public async Task<SubTask> CreateSubTaskAsync(SubTask subTask)
        {
            _context.SubTasks.Add(subTask);
            await _context.SaveChangesAsync();
            return subTask;
        }

        public Task<SubTask?> GetSubTaskAsync(int taskId, int subTaskId, string userId)
            => _context.SubTasks
                .Include(s => s.Task)
                .FirstOrDefaultAsync(s =>
                    s.Id == subTaskId && s.TaskId == taskId && s.Task.UserId == userId);

        public async Task<bool> DeleteSubTaskAsync(int taskId, int subTaskId, string userId)
        {
            var subTask = await GetSubTaskAsync(taskId, subTaskId, userId);
            if (subTask is null) return false;
            _context.SubTasks.Remove(subTask);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
