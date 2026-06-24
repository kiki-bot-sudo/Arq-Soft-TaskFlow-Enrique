using Microsoft.Extensions.Logging;
using Moq;
using TaskFlow.Application.Observers;
using TaskFlow.Infrastructure.Interfaces;
using TaskFlow.Domain.Models;
using Xunit;
using DomainTask = TaskFlow.Domain.Models.Task;
using SysTask = System.Threading.Tasks.Task;

namespace TaskFlow.Tests.Observers
{
    /// <summary>
    /// Tests para ActivityCompletionObserver.
    /// Cubre el patrón Observer: reacción automática al completar/descompletar tareas.
    /// </summary>
    public class ActivityCompletionObserverTests
    {
        private readonly Mock<ITaskRepository>     _taskRepoMock     = new();
        private readonly Mock<IActivityRepository> _activityRepoMock = new();
        private readonly Mock<ILogger<ActivityCompletionObserver>> _loggerMock = new();

        private ActivityCompletionObserver CreateObserver()
            => new(_taskRepoMock.Object, _activityRepoMock.Object, _loggerMock.Object);

        [Fact]
        public async SysTask OnTaskUpdated_AllTasksComplete_MarksActivityAsCompleted()
        {
            var task = new DomainTask { Id = 1, ActivityId = 10, IsCompleted = true };

            _taskRepoMock.Setup(r => r.GetTasksByActivityAsync(10))
                .ReturnsAsync(new List<DomainTask>
                {
                    new() { Id = 1, IsCompleted = true },
                    new() { Id = 2, IsCompleted = true }
                });

            var activity = new Activity { Id = 10, IsCompleted = false };
            _activityRepoMock.Setup(r => r.GetActivityByIdAsync(10)).ReturnsAsync(activity);
            _activityRepoMock.Setup(r => r.UpdateActivityAsync(It.IsAny<Activity>()))
                .ReturnsAsync((Activity a) => a);

            var observer = CreateObserver();
            await observer.OnTaskUpdatedAsync(task);

            _activityRepoMock.Verify(r =>
                r.UpdateActivityAsync(It.Is<Activity>(a => a.IsCompleted == true)), Times.Once);
        }

        [Fact]
        public async SysTask OnTaskUpdated_NotAllComplete_DoesNotMarkActivity()
        {
            var task = new DomainTask { Id = 1, ActivityId = 10, IsCompleted = true };

            _taskRepoMock.Setup(r => r.GetTasksByActivityAsync(10))
                .ReturnsAsync(new List<DomainTask>
                {
                    new() { Id = 1, IsCompleted = true },
                    new() { Id = 2, IsCompleted = false } // pendiente
                });

            var activity = new Activity { Id = 10, IsCompleted = false };
            _activityRepoMock.Setup(r => r.GetActivityByIdAsync(10)).ReturnsAsync(activity);

            var observer = CreateObserver();
            await observer.OnTaskUpdatedAsync(task);

            _activityRepoMock.Verify(r => r.UpdateActivityAsync(It.IsAny<Activity>()), Times.Never);
        }

        [Fact]
        public async SysTask OnTaskUpdated_TaskUncompleted_ReopensActivity()
        {
            var task = new DomainTask { Id = 1, ActivityId = 10, IsCompleted = false };

            _taskRepoMock.Setup(r => r.GetTasksByActivityAsync(10))
                .ReturnsAsync(new List<DomainTask>
                {
                    new() { Id = 1, IsCompleted = false },
                    new() { Id = 2, IsCompleted = true }
                });

            // Actividad que estaba completada
            var activity = new Activity { Id = 10, IsCompleted = true };
            _activityRepoMock.Setup(r => r.GetActivityByIdAsync(10)).ReturnsAsync(activity);
            _activityRepoMock.Setup(r => r.UpdateActivityAsync(It.IsAny<Activity>()))
                .ReturnsAsync((Activity a) => a);

            var observer = CreateObserver();
            await observer.OnTaskUpdatedAsync(task);

            // Debe reabrir la actividad
            _activityRepoMock.Verify(r =>
                r.UpdateActivityAsync(It.Is<Activity>(a => a.IsCompleted == false)), Times.Once);
        }

        [Fact]
        public async SysTask OnTaskUpdated_NoTasks_DoesNothing()
        {
            var task = new DomainTask { Id = 1, ActivityId = 10 };

            _taskRepoMock.Setup(r => r.GetTasksByActivityAsync(10))
                .ReturnsAsync(new List<DomainTask>());

            var observer = CreateObserver();
            await observer.OnTaskUpdatedAsync(task);

            _activityRepoMock.Verify(r => r.UpdateActivityAsync(It.IsAny<Activity>()), Times.Never);
        }

        [Fact]
        public async SysTask OnTaskUpdated_ActivityNotFound_DoesNotThrow()
        {
            var task = new DomainTask { Id = 1, ActivityId = 99 };

            _taskRepoMock.Setup(r => r.GetTasksByActivityAsync(99))
                .ReturnsAsync(new List<DomainTask> { new() { IsCompleted = true } });

            _activityRepoMock.Setup(r => r.GetActivityByIdAsync(99))
                .ReturnsAsync((Activity?)null);

            var observer = CreateObserver();
            var ex = await Record.ExceptionAsync(() => observer.OnTaskUpdatedAsync(task));
            Assert.Null(ex);
        }
    }
}
