using Xunit;
using TaskFlow.Application.Services;
using TaskFlow.Domain.Models;
using TaskFlow.Infrastructure.Interfaces;
using Moq;
using Task = TaskFlow.Domain.Models.Task;

namespace TaskFlow.Tests
{
    public class TaskServiceTests
    {
        private readonly Mock<ITaskRepository> _mockRepository;
        private readonly TaskService _service;

        public TaskServiceTests()
        {
            _mockRepository = new Mock<ITaskRepository>();
            _service = new TaskService(_mockRepository.Object);
        }

        [Fact]
        public async Task GetTasksByActivityAsync_ReturnsTasks_WhenActivityHasTasks()
        {
            // Arrange
            int activityId = 1;
            var tasks = new List<Task>
            {
                new Task { Id = 1, ActivityId = activityId, Title = "Tarea 1" },
                new Task { Id = 2, ActivityId = activityId, Title = "Tarea 2" }
            };
            _mockRepository.Setup(r => r.GetTasksByActivityAsync(activityId))
                .ReturnsAsync(tasks);

            // Act
            var result = await _service.GetTasksByActivityAsync(activityId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, t => Assert.Equal(activityId, t.ActivityId));
        }

        [Fact]
        public async Task CreateTaskAsync_SetsCreatedAtTimestamp()
        {
            // Arrange
            var task = new Task { Title = "Nueva tarea", ActivityId = 1 };
            _mockRepository.Setup(r => r.CreateTaskAsync(It.IsAny<Task>()))
                .ReturnsAsync(task);

            // Act
            var result = await _service.CreateTaskAsync(task);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(DateTime.MinValue, result.CreatedAt);
        }

        [Fact]
        public async Task UpdateTaskAsync_UpdatesTaskProperties()
        {
            // Arrange
            var task = new Task { Id = 1, Title = "Original", IsCompleted = false };
            task.Title = "Actualizado";
            task.IsCompleted = true;
            _mockRepository.Setup(r => r.UpdateTaskAsync(It.IsAny<Task>()))
                .ReturnsAsync(task);

            // Act
            var result = await _service.UpdateTaskAsync(task);

            // Assert
            Assert.True(result.IsCompleted);
            Assert.Equal("Actualizado", result.Title);
        }
    }
}
