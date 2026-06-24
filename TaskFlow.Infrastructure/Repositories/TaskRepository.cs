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
        public async Task<IEnumerable<Task>> GetTasksByActivityAsync(int activityId)
        {
            return await _context.Tasks
                .Where(t => t.ActivityId == activityId)
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene una tarea por ID con su actividad relacionada.
        /// </summary>
        /// <param name="id">ID de la tarea</param>
        /// <returns>Tarea con su actividad, o null si no existe</returns>
        public async Task<Task?> GetTaskByIdAsync(int id)
        {
            return await _context.Tasks
                .Include(t => t.Activity)
                .FirstOrDefaultAsync(t => t.Id == id);
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
        public async Task<bool> DeleteTaskAsync(int id)
        {
            var task = await GetTaskByIdAsync(id);
            if (task == null) return false;

            Delete(task);
            await SaveChangesAsync();
            return true;
        }
    }
}
