using Moq;
using TaskFlow.Application.Observers;
using TaskFlow.Application.Services;
using TaskFlow.Infrastructure.Interfaces;
using Xunit;
using DomainTask = TaskFlow.Domain.Models.Task;
using SysTask = System.Threading.Tasks.Task;

namespace TaskFlow.Tests.Services
{
    /// <summary>
    /// Tests para TaskService.
    /// Cubre el patrón Observer: notificación al actualizar tareas.
    /// </summary>
    public class TaskServiceTests
    {
        private readonly Mock<ITaskRepository> _repoMock = new();

        [Fact]
        public async SysTask UpdateTaskAsync_NotifiesAllObservers()
        {
            var task = new DomainTask { Id = 1, ActivityId = 1, Title = "Tarea", IsCompleted = true };
            _repoMock.Setup(r => r.UpdateTaskAsync(It.IsAny<DomainTask>()))
                     .ReturnsAsync((DomainTask t) => t);

            var observer1 = new Mock<ITaskObserver>();
            var observer2 = new Mock<ITaskObserver>();

            var service = new TaskService(_repoMock.Object);
            service.AddObserver(observer1.Object);
            service.AddObserver(observer2.Object);

            await service.UpdateTaskAsync(task);

            // Patrón Observer: ambos observadores deben ser notificados
            observer1.Verify(o => o.OnTaskUpdatedAsync(task), Times.Once);
            observer2.Verify(o => o.OnTaskUpdatedAsync(task), Times.Once);
        }

        [Fact]
        public async SysTask UpdateTaskAsync_NoObservers_DoesNotThrow()
        {
            var task = new DomainTask { Id = 1, ActivityId = 1, Title = "Tarea" };
            _repoMock.Setup(r => r.UpdateTaskAsync(It.IsAny<DomainTask>()))
                     .ReturnsAsync((DomainTask t) => t);

            var service = new TaskService(_repoMock.Object);

            var ex = await Record.ExceptionAsync(() => service.UpdateTaskAsync(task));
            Assert.Null(ex);
        }

        [Fact]
        public async SysTask RemoveObserver_StopsReceivingNotifications()
        {
            var task = new DomainTask { Id = 1, ActivityId = 1, Title = "Tarea" };
            _repoMock.Setup(r => r.UpdateTaskAsync(It.IsAny<DomainTask>()))
                     .ReturnsAsync((DomainTask t) => t);

            var observer = new Mock<ITaskObserver>();
            var service = new TaskService(_repoMock.Object);
            service.AddObserver(observer.Object);
            service.RemoveObserver(observer.Object);

            await service.UpdateTaskAsync(task);

            observer.Verify(o => o.OnTaskUpdatedAsync(It.IsAny<DomainTask>()), Times.Never);
        }

        [Fact]
        public async SysTask CreateTaskAsync_SetsCreatedAt()
        {
            _repoMock.Setup(r => r.CreateTaskAsync(It.IsAny<DomainTask>()))
                     .ReturnsAsync((DomainTask t) => t);

            var service = new TaskService(_repoMock.Object);
            var result = await service.CreateTaskAsync(new DomainTask { Title = "Test" });

            Assert.True(result.CreatedAt <= DateTime.UtcNow);
            Assert.True(result.CreatedAt > DateTime.UtcNow.AddSeconds(-5));
        }

        [Fact]
        public async SysTask DeleteTaskAsync_ReturnsFalse_WhenNotFound()
        {
            _repoMock.Setup(r => r.DeleteTaskAsync(99, "user-a")).ReturnsAsync(false);

            var service = new TaskService(_repoMock.Object);
            var result = await service.DeleteTaskAsync(99, "user-a");

            Assert.False(result);
        }

        [Fact]
        public async SysTask CreateTaskAsync_RejectsWhitespaceTitle()
        {
            var service = new TaskService(_repoMock.Object);

            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => service.CreateTaskAsync(new DomainTask { Title = "   " }));

            Assert.Equal("El título es obligatorio.", exception.Message);
            _repoMock.Verify(r => r.CreateTaskAsync(It.IsAny<DomainTask>()), Times.Never);
        }

        [Fact]
        public async SysTask GetTaskByIdAsync_UsesAuthenticatedUserId()
        {
            _repoMock.Setup(r => r.GetTaskByIdAsync(7, "user-a"))
                .ReturnsAsync(new DomainTask { Id = 7, UserId = "user-a", Title = "Propia" });
            var service = new TaskService(_repoMock.Object);

            var result = await service.GetTaskByIdAsync(7, "user-a");

            Assert.NotNull(result);
            _repoMock.Verify(r => r.GetTaskByIdAsync(7, "user-a"), Times.Once);
            _repoMock.Verify(r => r.GetTaskByIdAsync(7, "user-b"), Times.Never);
        }

        [Fact]
        public async SysTask CreateSubTaskAsync_ReturnsNull_WhenParentDoesNotBelongToUser()
        {
            _repoMock.Setup(r => r.GetTaskByIdAsync(9, "user-b"))
                .ReturnsAsync((DomainTask?)null);
            var service = new TaskService(_repoMock.Object);

            var result = await service.CreateSubTaskAsync(9, "Intento ajeno", "user-b");

            Assert.Null(result);
            _repoMock.Verify(r => r.CreateSubTaskAsync(It.IsAny<TaskFlow.Domain.Models.SubTask>()), Times.Never);
        }
    }
}
