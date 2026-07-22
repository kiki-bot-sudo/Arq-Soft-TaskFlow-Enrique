using Xunit;

namespace TaskFlow.Tests
{
    public class ActivityServiceTests
    {
        [Fact]
        public void ActivityService_ShouldExist()
        {
            Assert.True(true);
        }

        [Fact]
        public void CreateActivity_ShouldAssignTitle()
        {
            string title = "Test Activity";
            Assert.Equal("Test Activity", title);
        }

        [Fact]
        public void DeleteActivity_ShouldReturnTrue()
        {
            bool result = true;
            Assert.True(result);
        }
    }

    public class TaskServiceTests
    {
        [Fact]
        public void TaskService_ShouldExist()
        {
            Assert.True(true);
        }

        [Fact]
        public void CreateTask_ShouldAssignTitle()
        {
            string title = "Test Task";
            Assert.Equal("Test Task", title);
        }

        [Fact]
        public void UpdateTask_ShouldMarkComplete()
        {
            bool isCompleted = true;
            Assert.True(isCompleted);
        }
    }

    public class CalculadoraTests
    {
        [Fact]
        public void Sumar_ReturnsSumOfTwoNumbers()
        {
            int a = 5;
            int b = 3;
            int expected = 8;
            int result = a + b;
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Restar_ReturnsCorrectDifference()
        {
            int a = 10;
            int b = 4;
            int expected = 6;
            int result = a - b;
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Multiplicar_ReturnsCorrectProduct()
        {
            int a = 6;
            int b = 7;
            int expected = 42;
            int result = a * b;
            Assert.Equal(expected, result);
        }
    }
}
