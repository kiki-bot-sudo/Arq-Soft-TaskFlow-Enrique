using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.DTOs;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Models;

namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivityController : ControllerBase
    {
        private readonly IActivityService _activityService;

        public ActivityController(IActivityService activityService)
        {
            _activityService = activityService;
        }

        [HttpGet("today")]
        public async Task<IActionResult> GetTodayActivities()
        {
            var activities = await _activityService.GetTodayActivitiesAsync();
            return Ok(activities);
        }

        [HttpGet("date/{date}")]
        public async Task<IActionResult> GetActivitiesByDate(DateTime date)
        {
            var activities = await _activityService.GetActivitiesByDateAsync(date);
            return Ok(activities);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetActivityById(int id)
        {
            var activity = await _activityService.GetActivityByIdAsync(id);
            if (activity == null)
                return NotFound();

            return Ok(activity);
        }

        [HttpPost]
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

        [HttpPut("{id}")]
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivity(int id)
        {
            var result = await _activityService.DeleteActivityAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
