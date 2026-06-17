using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.DTOs;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Models;

namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/activity/{activityId}/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTasksByActivity(int activityId)
        {
            var tasks = await _taskService.GetTasksByActivityAsync(activityId);
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
                return NotFound();

            return Ok(task);
        }

        [HttpPost]
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

        [HttpPut("{id}")]
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var result = await _taskService.DeleteTaskAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
