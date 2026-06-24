using Microsoft.AspNetCore.Mvc;
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
            var tasks = await _taskService.GetTasksByActivityAsync(activityId);
            return Ok(tasks);
        }

        /// <summary>Obtiene una tarea por ID.</summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null) return NotFound();
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
                .WithDueTime(dto.DueTime)
                .Build();

            var created = await _taskService.CreateTaskAsync(task);
            return CreatedAtAction(nameof(GetTaskById), new { activityId, id = created.Id }, created);
        }

        /// <summary>Actualiza una tarea existente.</summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto dto)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null) return NotFound();

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.IsCompleted = dto.IsCompleted;
            task.DueTime = dto.DueTime;

            var updated = await _taskService.UpdateTaskAsync(task);
            return Ok(updated);
        }

        /// <summary>Elimina una tarea.</summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var result = await _taskService.DeleteTaskAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
