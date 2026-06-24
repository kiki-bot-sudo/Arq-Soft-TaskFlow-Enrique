using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.DTOs;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Models;
using Task = TaskFlow.Domain.Models.Task;

namespace TaskFlow.Api.Controllers
{
    /// <summary>
    /// Controlador para gestionar tareas dentro de una actividad.
    /// Proporciona endpoints para crear, leer, actualizar y eliminar tareas.
    /// Las tareas siempre están asociadas a una actividad.
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

        /// <summary>
        /// Obtiene todas las tareas asociadas a una actividad específica.
        /// </summary>
        /// <param name="activityId">ID de la actividad</param>
        /// <returns>Lista de tareas de la actividad</returns>
        /// <response code="200">Tareas obtenidas exitosamente</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTasksByActivity(int activityId)
        {
            var tasks = await _taskService.GetTasksByActivityAsync(activityId);
            return Ok(tasks);
        }

        /// <summary>
        /// Obtiene una tarea específica por su ID.
        /// </summary>
        /// <param name="id">ID de la tarea</param>
        /// <returns>Tarea con su información completa</returns>
        /// <response code="200">Tarea obtenida exitosamente</response>
        /// <response code="404">Tarea no encontrada</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
                return NotFound();

            return Ok(task);
        }

        /// <summary>
        /// Crea una nueva tarea dentro de una actividad.
        /// </summary>
        /// <param name="activityId">ID de la actividad a la que pertenecerá la tarea</param>
        /// <param name="dto">Datos de la tarea a crear</param>
        /// <returns>Tarea creada con su ID</returns>
        /// <response code="201">Tarea creada exitosamente</response>
        /// <response code="400">Datos inválidos o actividad no existe</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTask(int activityId, [FromBody] CreateTaskDto dto)
        {
            var task = new Task
            {
                ActivityId = activityId,
                Title = dto.Title,
                Description = dto.Description,
                DueTime = dto.DueTime
            };

            var created = await _taskService.CreateTaskAsync(task);
            return CreatedAtAction(nameof(GetTaskById), new { activityId, id = created.Id }, created);
        }

        /// <summary>
        /// Actualiza una tarea existente.
        /// </summary>
        /// <param name="id">ID de la tarea a actualizar</param>
        /// <param name="dto">Datos actualizados de la tarea</param>
        /// <returns>Tarea actualizada</returns>
        /// <response code="200">Tarea actualizada exitosamente</response>
        /// <response code="404">Tarea no encontrada</response>
        /// <response code="400">Datos inválidos</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto dto)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
                return NotFound();

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.IsCompleted = dto.IsCompleted;
            task.DueTime = dto.DueTime;

            var updated = await _taskService.UpdateTaskAsync(task);
            return Ok(updated);
        }

        /// <summary>
        /// Elimina una tarea.
        /// </summary>
        /// <param name="id">ID de la tarea a eliminar</param>
        /// <response code="204">Tarea eliminada exitosamente</response>
        /// <response code="404">Tarea no encontrada</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var result = await _taskService.DeleteTaskAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
