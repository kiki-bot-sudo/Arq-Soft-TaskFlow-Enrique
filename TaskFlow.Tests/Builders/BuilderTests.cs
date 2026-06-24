using TaskFlow.Domain.Builders;
using Xunit;

namespace TaskFlow.Tests.Builders
{
    /// <summary>
    /// Tests para ActivityBuilder y TaskBuilder.
    /// Cubre el patrón Builder: construcción paso a paso con valores por defecto.
    /// </summary>
    public class BuilderTests
    {
        // ── ActivityBuilder ──────────────────────────────────────────

        [Fact]
        public void ActivityBuilder_Build_SetsAllProperties()
        {
            var date = new DateTime(2026, 6, 24);
            var activity = new ActivityBuilder()
                .WithTitle("Estudiar")
                .WithDescription("GoF patterns")
                .WithDate(date)
                .WithCategory("Universidad")
                .WithPriority("High")
                .Build();

            Assert.Equal("Estudiar",     activity.Title);
            Assert.Equal("GoF patterns", activity.Description);
            Assert.Equal(date,           activity.Date);
            Assert.Equal("Universidad",  activity.Category);
            Assert.Equal("High",         activity.Priority);
        }

        [Fact]
        public void ActivityBuilder_Build_SetsCreatedAtAutomatically()
        {
            var before = DateTime.UtcNow;
            var activity = new ActivityBuilder().WithTitle("Test").Build();
            var after = DateTime.UtcNow;

            Assert.InRange(activity.CreatedAt, before, after);
        }

        [Fact]
        public void ActivityBuilder_Build_DefaultIsCompletedFalse()
        {
            var activity = new ActivityBuilder().WithTitle("Test").Build();
            Assert.False(activity.IsCompleted);
        }

        [Fact]
        public void ActivityBuilder_IsFluentInterface_ReturnsSameInstance()
        {
            var builder = new ActivityBuilder();
            var result = builder.WithTitle("X");
            Assert.Same(builder, result);
        }

        // ── TaskBuilder ──────────────────────────────────────────────

        [Fact]
        public void TaskBuilder_Build_SetsAllProperties()
        {
            var due = new DateTime(2026, 6, 25, 10, 0, 0);
            var task = new TaskBuilder()
                .WithActivityId(5)
                .WithTitle("Commit changes")
                .WithDescription("Push to api branch")
                .WithDueTime(due)
                .Build();

            Assert.Equal(5,                 task.ActivityId);
            Assert.Equal("Commit changes",  task.Title);
            Assert.Equal("Push to api branch", task.Description);
            Assert.Equal(due,               task.DueTime);
        }

        [Fact]
        public void TaskBuilder_Build_SetsCreatedAtAutomatically()
        {
            var before = DateTime.UtcNow;
            var task = new TaskBuilder().WithTitle("Test").Build();
            var after = DateTime.UtcNow;

            Assert.InRange(task.CreatedAt, before, after);
        }

        [Fact]
        public void TaskBuilder_Build_DefaultIsCompletedFalse()
        {
            var task = new TaskBuilder().WithTitle("Test").Build();
            Assert.False(task.IsCompleted);
        }

        [Fact]
        public void TaskBuilder_WithNullDueTime_IsAccepted()
        {
            var task = new TaskBuilder().WithTitle("No deadline").WithDueTime(null).Build();
            Assert.Null(task.DueTime);
        }
    }
}
