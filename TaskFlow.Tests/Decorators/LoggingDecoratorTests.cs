using Microsoft.Extensions.Logging;
using Moq;
using TaskFlow.Application.Decorators;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Models;
using Xunit;

namespace TaskFlow.Tests.Decorators
{
    /// <summary>
    /// Tests para LoggingActivityServiceDecorator.
    /// Verifica que el Decorator delega correctamente al servicio interno
    /// y que no altera el resultado.
    /// </summary>
    public class LoggingDecoratorTests
    {
        private readonly Mock<IActivityService> _innerMock = new();
        private readonly Mock<ILogger<LoggingActivityServiceDecorator>> _loggerMock = new();

        private LoggingActivityServiceDecorator CreateDecorator()
            => new(_innerMock.Object, _loggerMock.Object);

        [Fact]
        public async System.Threading.Tasks.Task GetActivitiesByDateAsync_DelegatesToInnerService()
        {
            var date = DateTime.Today;
            var expected = new List<Activity> { new() { Id = 1, Title = "Test" } };
            _innerMock.Setup(s => s.GetActivitiesByDateAsync(date)).ReturnsAsync(expected);

            var decorator = CreateDecorator();
            var result = await decorator.GetActivitiesByDateAsync(date);

            Assert.Equal(expected, result);
            _innerMock.Verify(s => s.GetActivitiesByDateAsync(date), Times.Once);
        }

        [Fact]
        public async System.Threading.Tasks.Task CreateActivityAsync_DelegatesToInnerAndReturnsResult()
        {
            var activity = new Activity { Title = "Nueva actividad" };
            var created  = new Activity { Id = 1, Title = "Nueva actividad" };
            _innerMock.Setup(s => s.CreateActivityAsync(activity)).ReturnsAsync(created);

            var decorator = CreateDecorator();
            var result = await decorator.CreateActivityAsync(activity);

            Assert.Equal(created.Id,    result.Id);
            Assert.Equal(created.Title, result.Title);
        }

        [Fact]
        public async System.Threading.Tasks.Task DeleteActivityAsync_DelegatesToInner()
        {
            _innerMock.Setup(s => s.DeleteActivityAsync(1)).ReturnsAsync(true);

            var decorator = CreateDecorator();
            var result = await decorator.DeleteActivityAsync(1);

            Assert.True(result);
            _innerMock.Verify(s => s.DeleteActivityAsync(1), Times.Once);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetTodayActivitiesAsync_DelegatesToInner()
        {
            var expected = new List<Activity> { new() { Id = 2 } };
            _innerMock.Setup(s => s.GetTodayActivitiesAsync()).ReturnsAsync(expected);

            var decorator = CreateDecorator();
            var result = await decorator.GetTodayActivitiesAsync();

            Assert.Single(result);
        }

        [Fact]
        public async System.Threading.Tasks.Task UpdateActivityAsync_DelegatesToInner()
        {
            var activity = new Activity { Id = 1, Title = "Upd" };
            _innerMock.Setup(s => s.UpdateActivityAsync(activity)).ReturnsAsync(activity);

            var decorator = CreateDecorator();
            var result = await decorator.UpdateActivityAsync(activity);

            Assert.Equal(activity, result);
        }
    }
}
