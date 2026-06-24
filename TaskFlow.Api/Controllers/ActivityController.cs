using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.DTOs;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Builders;
using TaskFlow.Domain.Models;

namespace TaskFlow.Api.Controllers
{
    /// <summary>
    /// Controlador para gestionar actividades.
    /// Usa el patrón Builder para construir instancias de Activity desde los DTOs.
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

        /// <summary>Obtiene todas las actividades programadas para hoy.</summary>
        [HttpGet("today")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTodayActivities()
        {
            var activities = await _activityService.GetTodayActivitiesAsync();
            return Ok(activities);
        }

        /// <summary>Obtiene las actividades para una fecha específica.</summary>
        [HttpGet("date/{date}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActivitiesByDate(DateTime date)
        {
            var activities = await _activityService.GetActivitiesByDateAsync(date);
            return Ok(activities);
        }

        /// <summary>Obtiene una actividad específica por su ID.</summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetActivityById(int id)
        {
            var activity = await _activityService.GetActivityByIdAsync(id);
            if (activity == null) return NotFound();
            return Ok(activity);
        }

        /// <summary>
        /// Crea una nueva actividad.
        /// Usa ActivityBuilder (GoF Builder) para construir la entidad desde el DTO.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateActivity([FromBody] CreateActivityDto dto)
        {
            var activity = new ActivityBuilder()
                .WithTitle(dto.Title)
                .WithDescription(dto.Description)
                .WithDate(dto.Date)
                .WithCategory(dto.Category)
                .WithPriority(dto.Priority ?? "Normal")
                .Build();

            var created = await _activityService.CreateActivityAsync(activity);
            return CreatedAtAction(nameof(GetActivityById), new { id = created.Id }, created);
        }

        /// <summary>Actualiza una actividad existente.</summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateActivity(int id, [FromBody] UpdateActivityDto dto)
        {
            var activity = await _activityService.GetActivityByIdAsync(id);
            if (activity == null) return NotFound();

            activity.Title = dto.Title;
            activity.Description = dto.Description;
            activity.Category = dto.Category;
            activity.Priority = dto.Priority ?? activity.Priority;
            activity.IsCompleted = dto.IsCompleted;

            var updated = await _activityService.UpdateActivityAsync(activity);
            return Ok(updated);
        }

        /// <summary>Elimina una actividad y sus tareas asociadas.</summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteActivity(int id)
        {
            var result = await _activityService.DeleteActivityAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
