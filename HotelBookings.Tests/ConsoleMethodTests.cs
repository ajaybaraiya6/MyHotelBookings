using HotelBookings.BusinessCore;
namespace HotelBookings.Tests
{
    public class ConsoleMethodTests
    {
        [Theory]
        [InlineData("Availability(H1, 20250401-20250410, SGL)", OperationCommands.Availability)]
        [InlineData("Search(H1, 365, SGL)", OperationCommands.Search)]
        [InlineData("exit", OperationCommands.None)]
        public void GetOperationMethodFromUser_ShouldReturnCorrectEnum_WhenGivenValidConsoleInput(string input, OperationCommands expected)
        {
            // Arrange
            using var inputReader = new StringReader(input + Environment.NewLine);
            using var outputWriter = new StringWriter();

            Console.SetIn(inputReader);     // Fake Console.ReadLine
            Console.SetOut(outputWriter);   // Suppress Console.WriteLine

            // Act
            string commandString;
            var result = (OperationCommands)Common.GetOperationMethodFromUser(out commandString);

            // Assert
            Assert.Equal(expected, result);
            if (expected != OperationCommands.None)
            {
                Assert.Equal(input.Trim(), commandString);
            }
        }
    }
}

