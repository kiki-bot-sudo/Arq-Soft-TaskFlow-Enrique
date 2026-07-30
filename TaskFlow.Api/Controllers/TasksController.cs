using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TaskFlow.Api.DTOs;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Builders;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService) => _taskService = taskService;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] bool? isCompleted,
        [FromQuery] string? priority,
        [FromQuery] DateTime? dueDate,
        [FromQuery] string sortBy = "date",
        [FromQuery] bool descending = false)
    {
        if (priority is not null && priority is not ("Low" or "Medium" or "High"))
        {
            ModelState.AddModelError("priority", "La prioridad debe ser Low, Medium o High.");
            return ValidationProblem(ModelState);
        }
        if (sortBy is not ("date" or "priority" or "name"))
        {
            ModelState.AddModelError("sortBy", "El orden debe ser date, priority o name.");
            return ValidationProblem(ModelState);
        }

        return Ok(await _taskService.GetTasksAsync(
            UserId, search, isCompleted, priority, dueDate, sortBy, descending));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var task = await _taskService.GetTaskByIdAsync(id, UserId);
        return task is null ? NotFound() : Ok(task);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskDto dto)
    {
        var task = new TaskBuilder()
            .WithTitle(dto.Title)
            .WithDescription(dto.Description)
            .WithPriority(dto.Priority)
            .WithDueTime(dto.DueTime)
            .Build();
        task.UserId = UserId;
        var created = await _taskService.CreateTaskAsync(task);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateTaskDto dto)
    {
        var task = await _taskService.GetTaskByIdAsync(id, UserId);
        if (task is null) return NotFound();
        task.Title = dto.Title;
        task.Description = dto.Description;
        task.Priority = dto.Priority;
        task.DueTime = dto.DueTime;
        task.IsCompleted = dto.IsCompleted;
        return Ok(await _taskService.UpdateTaskAsync(task));
    }

    [HttpPatch("{id:int}/completion")]
    public async Task<IActionResult> SetCompletion(int id, CompletionDto dto)
    {
        var task = await _taskService.SetCompletionAsync(id, dto.IsCompleted, UserId);
        return task is null ? NotFound() : Ok(task);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
        => await _taskService.DeleteTaskAsync(id, UserId) ? NoContent() : NotFound();

    [HttpGet("statistics")]
    public async Task<IActionResult> Statistics()
        => Ok(await _taskService.GetStatisticsAsync(UserId));

    [HttpPost("{taskId:int}/subtasks")]
    public async Task<IActionResult> CreateSubTask(int taskId, SubTaskDto dto)
    {
        var subTask = await _taskService.CreateSubTaskAsync(taskId, dto.Title, UserId);
        return subTask is null ? NotFound() : Ok(subTask);
    }

    [HttpPatch("{taskId:int}/subtasks/{subTaskId:int}")]
    public async Task<IActionResult> CompleteSubTask(
        int taskId, int subTaskId, CompletionDto dto)
    {
        var subTask = await _taskService.SetSubTaskCompletionAsync(
            taskId, subTaskId, dto.IsCompleted, UserId);
        return subTask is null ? NotFound() : Ok(subTask);
    }

    [HttpDelete("{taskId:int}/subtasks/{subTaskId:int}")]
    public async Task<IActionResult> DeleteSubTask(int taskId, int subTaskId)
        => await _taskService.DeleteSubTaskAsync(taskId, subTaskId, UserId)
            ? NoContent()
            : NotFound();

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new InvalidOperationException("No se encontró el usuario autenticado.");
}

public record CompletionDto(bool IsCompleted);
public record SubTaskDto(string Title);
