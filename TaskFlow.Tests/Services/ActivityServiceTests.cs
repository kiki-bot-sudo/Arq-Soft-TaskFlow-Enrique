using Moq;
using TaskFlow.Application.Services;
using TaskFlow.Application.Strategies;
using TaskFlow.Domain.Models;
using TaskFlow.Infrastructure.Interfaces;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace TaskFlow.Tests.Services
{
    /// <summary>
    /// Tests para ActivityService.
    /// Cubre el patrón Strategy y la lógica de negocio del servicio.
    /// </summary>
    public class ActivityServiceTests
    {
        private readonly Mock<IActivityRepository> _repoMock = new();

        private List<Activity> SampleActivities() => new()
        {
            new Activity { Id = 1, Title = "Low",    Priority = "Low",    Date = DateTime.Today },
            new Activity { Id = 2, Title = "High",   Priority = "High",   Date = DateTime.Today.AddHours(3) },
            new Activity { Id = 3, Title = "Normal", Priority = "Normal", Date = DateTime.Today.AddHours(1) },
        };

        [Fact]
        public async Task GetActivitiesByDateAsync_DefaultStrategy_SortsByPriorityDesc()
        {
            _repoMock.Setup(r => r.GetActivitiesByDateAsync(It.IsAny<DateTime>()))
                     .ReturnsAsync(SampleActivities());

            var service = new ActivityService(_repoMock.Object);
            var result = (await service.GetActivitiesByDateAsync(DateTime.Today)).ToList();

            Assert.Equal("High",   result[0].Title);
            Assert.Equal("Normal", result[1].Title);
            Assert.Equal("Low",    result[2].Title);
        }

        [Fact]
        public async Task GetActivitiesByDateAsync_DateStrategy_SortsByDateAsc()
        {
            _repoMock.Setup(r => r.GetActivitiesByDateAsync(It.IsAny<DateTime>()))
                     .ReturnsAsync(SampleActivities());

            var service = new ActivityService(_repoMock.Object);
            // Patrón Strategy: cambiar algoritmo en runtime
            service.SetSortStrategy(new DateAscSortStrategy());

            var result = (await service.GetActivitiesByDateAsync(DateTime.Today)).ToList();

            Assert.Equal("Low",    result[0].Title); // Today (earliest)
            Assert.Equal("Normal", result[1].Title); // Today +1h
            Assert.Equal("High",   result[2].Title); // Today +3h
        }

        [Fact]
        public async Task CreateActivityAsync_SetsCreatedAt()
        {
            var activity = new Activity { Title = "Test" };
            _repoMock.Setup(r => r.CreateActivityAsync(It.IsAny<Activity>()))
                     .ReturnsAsync((Activity a) => a);

            var service = new ActivityService(_repoMock.Object);
            var result = await service.CreateActivityAsync(activity);

            Assert.True(result.CreatedAt <= DateTime.UtcNow);
            Assert.True(result.CreatedAt > DateTime.UtcNow.AddSeconds(-5));
        }

        [Fact]
        public async Task GetActivityByIdAsync_ReturnsNull_WhenNotFound()
        {
            _repoMock.Setup(r => r.GetActivityByIdAsync(99))
                     .ReturnsAsync((Activity?)null);

            var service = new ActivityService(_repoMock.Object);
            var result = await service.GetActivityByIdAsync(99);

            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteActivityAsync_ReturnsTrue_WhenExists()
        {
            _repoMock.Setup(r => r.DeleteActivityAsync(1)).ReturnsAsync(true);

            var service = new ActivityService(_repoMock.Object);
            var result = await service.DeleteActivityAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task UpdateActivityAsync_SetsUpdatedAt()
        {
            var activity = new Activity { Id = 1, Title = "Updated" };
            _repoMock.Setup(r => r.UpdateActivityAsync(It.IsAny<Activity>()))
                     .ReturnsAsync((Activity a) => a);

            var service = new ActivityService(_repoMock.Object);
            var result = await service.UpdateActivityAsync(activity);

            Assert.NotNull(result.UpdatedAt);
            Assert.True(result.UpdatedAt <= DateTime.UtcNow);
        }
    }
}
