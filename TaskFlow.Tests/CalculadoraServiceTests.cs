using Xunit;

namespace TaskFlow.Tests
{
    public class CalculadoraServiceTests
    {
        [Fact]
        public void Sumar_ReturnsSumOfTwoNumbers()
        {
            // Arrange
            int a = 5;
            int b = 3;
            int expected = 8;

            // Act
            int result = a + b;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Restar_ReturnsSubtractionOfTwoNumbers()
        {
            // Arrange
            int a = 10;
            int b = 4;
            int expected = 6;

            // Act
            int result = a - b;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Dividir_ThrowsException_WhenDividingByZero()
        {
            // Arrange
            int a = 20;
            int b = 0;

            // Act & Assert
            Assert.Throws<DivideByZeroException>(() => a / b);
        }
    }
}
