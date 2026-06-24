using TaskFlow.Infrastructure.Interfaces;
using TaskFlow.Application.Interfaces;

namespace TaskFlow.Application.Observers
{
    /// <summary>
    /// GoF - Patrón Observer: Observador concreto
    /// Reacciona cuando una tarea es actualizada.
    /// Si todas las tareas de la actividad están completadas,
    /// marca automáticamente la actividad como completada.
    /// </summary>
    public class ActivityCompletionObserver : ITaskObserver
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IActivityRepository _activityRepository;
        private readonly ILogger<ActivityCompletionObserver> _logger;

        public ActivityCompletionObserver(
            ITaskRepository taskRepository,
            IActivityRepository activityRepository,
            ILogger<ActivityCompletionObserver> logger)
        {
            _taskRepository = taskRepository;
            _activityRepository = activityRepository;
            _logger = logger;
        }

        public async Task OnTaskUpdatedAsync(TaskFlow.Domain.Models.Task task)
        {
            var allTasks = await _taskRepository.GetTasksByActivityAsync(task.ActivityId);
            var taskList = allTasks.ToList();

            if (taskList.Count == 0) return;

            bool allCompleted = taskList.All(t => t.IsCompleted);

            var activity = await _activityRepository.GetActivityByIdAsync(task.ActivityId);
            if (activity == null) return;

            if (allCompleted && !activity.IsCompleted)
            {
                activity.IsCompleted = true;
                await _activityRepository.UpdateActivityAsync(activity);
                _logger.LogInformation(
                    "[Observer] Activity {Id} auto-completed: all {Count} tasks done.",
                    activity.Id, taskList.Count);
            }
            else if (!allCompleted && activity.IsCompleted)
            {
                activity.IsCompleted = false;
                await _activityRepository.UpdateActivityAsync(activity);
                _logger.LogInformation(
                    "[Observer] Activity {Id} re-opened: a task was un-completed.",
                    activity.Id);
            }
        }
    }
}
