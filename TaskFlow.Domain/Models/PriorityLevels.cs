namespace TaskFlow.Domain.Models
{
    /// <summary>
    /// Centraliza los valores válidos de prioridad de una actividad.
    /// Antes del refactor, el string mágico "Low|Normal|High" estaba duplicado
    /// en Activity, CreateActivityDto, UpdateActivityDto y PriorityDescSortStrategy,
    /// sin una única fuente de verdad (ver ADR-04).
    /// Se mantiene como string (no enum) para no requerir una nueva migración de EF,
    /// ya que la columna Priority ya existe como nvarchar(10) en la base de datos.
    /// </summary>
    public static class PriorityLevels
    {
        public const string Low = "Low";
        public const string Normal = "Normal";
        public const string High = "High";

        /// <summary>Expresión regular usada por los DTOs para validar el valor recibido.</summary>
        public const string ValidationPattern = "^(Low|Normal|High)$";

        /// <summary>Orden de clasificación usado por PriorityDescSortStrategy (0 = más alta).</summary>
        public static readonly IReadOnlyDictionary<string, int> SortOrder = new Dictionary<string, int>
        {
            { High, 0 },
            { Normal, 1 },
            { Low, 2 }
        };
    }
}
