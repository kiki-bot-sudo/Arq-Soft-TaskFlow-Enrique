using Xunit;
using TaskFlow.Application.Services;
using TaskFlow.Domain.Models;
using TaskFlow.Infrastructure.Interfaces;
using Moq;

namespace TaskFlow.Tests
{
    public class ActivityServiceTests
    {
        private readonly Mock<IActivityRepository> _mockRepository;
        private readonly ActivityService _service;

        public ActivityServiceTests()
        {
            _mockRepository = new Mock<IActivityRepository>();
            _service = new ActivityService(_mockRepository.Object);
        }

        [Fact]
        public async Task GetTodayActivitiesAsync_ReturnsActivities_WhenDataExists()
        {
            // Arrange
            var today = DateTime.UtcNow.Date;
            var activities = new List<Activity>
            {
                new Activity { Id = 1, Title = "Estudiar", Date = today },
                new Activity { Id = 2, Title = "Ejercicio", Date = today }
            };
            _mockRepository.Setup(r => r.GetActivitiesByDateAsync(It.IsAny<DateTime>()))
                .ReturnsAsync(activities);

            // Act
            var result = await _service.GetTodayActivitiesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockRepository.Verify(r => r.GetActivitiesByDateAsync(today), Times.Once);
        }

        [Fact]
        public async Task CreateActivityAsync_CreatesActivity_WithTimestamp()
        {
            // Arrange
            var activity = new Activity { Title = "Nueva actividad", Description = "Test" };
            _mockRepository.Setup(r => r.CreateActivityAsync(It.IsAny<Activity>()))
                .ReturnsAsync(activity);

            // Act
            var result = await _service.CreateActivityAsync(activity);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Nueva actividad", result.Title);
            Assert.NotEqual(DateTime.MinValue, result.CreatedAt);
        }

        [Fact]
        public async Task DeleteActivityAsync_ReturnsFalse_WhenActivityNotFound()
        {
            // Arrange
            _mockRepository.Setup(r => r.DeleteActivityAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            // Act
            var result = await _service.DeleteActivityAsync(999);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(r => r.DeleteActivityAsync(999), Times.Once);
        }
    }
}
