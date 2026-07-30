using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TaskFlow.Api.DTOs;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Builders;
using Task = TaskFlow.Domain.Models.Task;

namespace TaskFlow.Api.Controllers
{
    /// <summary>
    /// Controlador para gestionar tareas dentro de una actividad.
    /// Usa el patrón Builder para construir instancias de Task desde los DTOs.
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/activity/{activityId}/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        /// <summary>Obtiene todas las tareas de una actividad.</summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTasksByActivity(int activityId)
        {
            var tasks = await _taskService.GetTasksByActivityAsync(activityId, UserId);
            return Ok(tasks);
        }

        /// <summary>Obtiene una tarea por ID.</summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTaskById(int activityId, int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id, UserId);
            if (task == null || task.ActivityId != activityId) return NotFound();
            return Ok(task);
        }

        /// <summary>
        /// Crea una nueva tarea dentro de una actividad.
        /// Usa TaskBuilder (GoF Builder) para construir la entidad desde el DTO.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTask(int activityId, [FromBody] CreateTaskDto dto)
        {
            var task = new TaskBuilder()
                .WithActivityId(activityId)
                .WithTitle(dto.Title)
                .WithDescription(dto.Description)
                .WithPriority(dto.Priority)
                .WithDueTime(dto.DueTime)
                .Build();
            task.UserId = UserId;

            var created = await _taskService.CreateTaskAsync(task);
            return CreatedAtAction(nameof(GetTaskById), new { activityId, id = created.Id }, created);
        }

        /// <summary>Actualiza una tarea existente.</summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTask(int activityId, int id, [FromBody] UpdateTaskDto dto)
        {
            var task = await _taskService.GetTaskByIdAsync(id, UserId);
            if (task == null || task.ActivityId != activityId) return NotFound();

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.Priority = dto.Priority;
            task.IsCompleted = dto.IsCompleted;
            task.DueTime = dto.DueTime;

            var updated = await _taskService.UpdateTaskAsync(task);
            return Ok(updated);
        }

        /// <summary>Elimina una tarea.</summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTask(int activityId, int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id, UserId);
            if (task == null || task.ActivityId != activityId) return NotFound();
            var result = await _taskService.DeleteTaskAsync(id, UserId);
            if (!result) return NotFound();
            return NoContent();
        }

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("No se encontró el usuario autenticado.");
    }
}
