using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.DTOs;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Models;

namespace TaskFlow.Api.Controllers
{
    /// <summary>
    /// Controlador para gestionar actividades.
    /// Proporciona endpoints para crear, leer, actualizar y eliminar actividades.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ActivityController : ControllerBase
    {
        private readonly IActivityService _activityService;

        public ActivityController(IActivityService activityService)
        {
            _activityService = activityService;
        }

        /// <summary>
        /// Obtiene todas las actividades programadas para hoy.
        /// </summary>
        /// <returns>Lista de actividades del día actual</returns>
        /// <response code="200">Actividades obtenidas exitosamente</response>
        [HttpGet("today")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTodayActivities()
        {
            var activities = await _activityService.GetTodayActivitiesAsync();
            return Ok(activities);
        }

        /// <summary>
        /// Obtiene las actividades para una fecha específica.
        /// </summary>
        /// <param name="date">Fecha en formato ISO 8601 (yyyy-MM-dd)</param>
        /// <returns>Lista de actividades para la fecha especificada</returns>
        /// <response code="200">Actividades obtenidas exitosamente</response>
        [HttpGet("date/{date}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActivitiesByDate(DateTime date)
        {
            var activities = await _activityService.GetActivitiesByDateAsync(date);
            return Ok(activities);
        }

        /// <summary>
        /// Obtiene una actividad específica por su ID.
        /// </summary>
        /// <param name="id">ID de la actividad</param>
        /// <returns>Actividad con sus tareas asociadas</returns>
        /// <response code="200">Actividad obtenida exitosamente</response>
        /// <response code="404">Actividad no encontrada</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetActivityById(int id)
        {
            var activity = await _activityService.GetActivityByIdAsync(id);
            if (activity == null)
                return NotFound();

            return Ok(activity);
        }

        /// <summary>
        /// Crea una nueva actividad.
        /// </summary>
        /// <param name="dto">Datos de la actividad a crear</param>
        /// <returns>Actividad creada con su ID</returns>
        /// <response code="201">Actividad creada exitosamente</response>
        /// <response code="400">Datos inválidos</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateActivity([FromBody] CreateActivityDto dto)
        {
            var activity = new Activity
            {
                Title = dto.Title,
                Description = dto.Description,
                Date = dto.Date,
                Category = dto.Category,
                Priority = dto.Priority ?? "Normal"
            };

            var created = await _activityService.CreateActivityAsync(activity);
            return CreatedAtAction(nameof(GetActivityById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Actualiza una actividad existente.
        /// </summary>
        /// <param name="id">ID de la actividad a actualizar</param>
        /// <param name="dto">Datos actualizados de la actividad</param>
        /// <returns>Actividad actualizada</returns>
        /// <response code="200">Actividad actualizada exitosamente</response>
        /// <response code="404">Actividad no encontrada</response>
        /// <response code="400">Datos inválidos</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateActivity(int id, [FromBody] UpdateActivityDto dto)
        {
            var activity = await _activityService.GetActivityByIdAsync(id);
            if (activity == null)
                return NotFound();

            activity.Title = dto.Title;
            activity.Description = dto.Description;
            activity.Category = dto.Category;
            activity.Priority = dto.Priority ?? activity.Priority;
            activity.IsCompleted = dto.IsCompleted;

            var updated = await _activityService.UpdateActivityAsync(activity);
            return Ok(updated);
        }

        /// <summary>
        /// Elimina una actividad y sus tareas asociadas.
        /// </summary>
        /// <param name="id">ID de la actividad a eliminar</param>
        /// <response code="204">Actividad eliminada exitosamente</response>
        /// <response code="404">Actividad no encontrada</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteActivity(int id)
        {
            var result = await _activityService.DeleteActivityAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
