using TaskFlow.Domain.Models;

namespace TaskFlow.Domain.Builders
{
    /// <summary>
    /// GoF - Patrón Builder
    /// Construye objetos Activity paso a paso, aplicando valores por defecto
    /// y encadenando propiedades de forma legible.
    /// Evita constructores largos y reduce errores al crear actividades.
    /// </summary>
    public class ActivityBuilder
    {
        private readonly Activity _activity = new();

        public ActivityBuilder WithTitle(string title)
        {
            _activity.Title = title;
            return this;
        }

        public ActivityBuilder WithDescription(string description)
        {
            _activity.Description = description;
            return this;
        }

        public ActivityBuilder WithDate(DateTime date)
        {
            _activity.Date = date;
            return this;
        }

        public ActivityBuilder WithCategory(string category)
        {
            _activity.Category = category;
            return this;
        }

        public ActivityBuilder WithPriority(string priority)
        {
            _activity.Priority = priority;
            return this;
        }

        /// <summary>
        /// Finaliza la construcción y retorna la Activity lista para usar.
        /// CreatedAt se asigna automáticamente.
        /// </summary>
        public Activity Build()
        {
            _activity.CreatedAt = DateTime.UtcNow;
            return _activity;
        }
    }
}
