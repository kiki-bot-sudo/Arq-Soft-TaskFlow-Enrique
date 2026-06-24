using Microsoft.EntityFrameworkCore;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Infrastructure.Repositories
{
    /// <summary>
    /// Clase base para todos los repositorios genéricos.
    /// Proporciona operaciones CRUD comunes y gestión de cambios.
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    public abstract class BaseRepository<T> where T : class
    {
        protected readonly TaskFlowDbContext _context;

        protected BaseRepository(TaskFlowDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Obtiene todos los registros de la entidad.
        /// </summary>
        public virtual async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        /// <summary>
        /// Obtiene un registro por ID.
        /// </summary>
        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        /// <summary>
        /// Agrega una nueva entidad.
        /// </summary>
        public virtual async Task<T> AddAsync(T entity)
        {
            var result = await _context.Set<T>().AddAsync(entity);
            return result.Entity;
        }

        /// <summary>
        /// Actualiza una entidad existente.
        /// </summary>
        public virtual void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }

        /// <summary>
        /// Elimina una entidad.
        /// </summary>
        public virtual void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        /// <summary>
        /// Guarda todos los cambios en la base de datos.
        /// </summary>
        public virtual async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
