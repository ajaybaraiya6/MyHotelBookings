using Xunit;
using System;
using System.IO;
using HotelBookings.BusinessCore;

namespace HotelBookings.Tests
{
    public class CommonTests
    {
        [Fact]
        public void IsValidUserInput_ReturnsTrue_WhenBothFilesAreProvided()
        {
            // Arrange
            string hotelFile = null;
            string bookingFile = null;
            string[] args = new[] { "--hotels", "hotels.json", "--bookings", "bookings.json" };

            var originalOut = Console.Out;
            var originalIn = Console.In;
            using var writer = new StringWriter();
            using var reader = new StringReader(string.Empty);  // Mock an empty line for ReadLine
            Console.SetOut(writer);
            Console.SetIn(reader);

            // Act
            bool result = Common.IsValidUserInput(args, ref hotelFile, ref bookingFile);

            // Reset the console output and input
            Console.SetOut(originalOut);
            Console.SetIn(originalIn);

            // Assert
            Assert.True(result);
            Assert.Equal("hotels.json", hotelFile);
            Assert.Equal("bookings.json", bookingFile);
        }

        [Fact]
        public void IsValidUserInput_ReturnsFalse_WhenHotelFileIsMissing()
        {
            // Arrange
            string hotelFile = null;
            string bookingFile = null;
            string[] args = new[] { "--bookings", "bookings.json" };

            var originalOut = Console.Out;
            var originalIn = Console.In;
            using var writer = new StringWriter();
            using var reader = new StringReader(string.Empty);  // Mock an empty line for ReadLine
            Console.SetOut(writer);
            Console.SetIn(reader);

            // Act
            bool result = Common.IsValidUserInput(args, ref hotelFile, ref bookingFile);

            // Reset the console output and input
            Console.SetOut(originalOut);
            Console.SetIn(originalIn);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidUserInput_ReturnsFalse_WhenBookingFileIsMissing()
        {
            // Arrange
            string hotelFile = null;
            string bookingFile = null;
            string[] args = new[] { "--hotels", "hotels.json" };

            var originalOut = Console.Out;
            var originalIn = Console.In;
            using var writer = new StringWriter();
            using var reader = new StringReader(string.Empty);  // Mock an empty line for ReadLine
            Console.SetOut(writer);
            Console.SetIn(reader);

            // Act
            bool result = Common.IsValidUserInput(args, ref hotelFile, ref bookingFile);

            // Reset the console output and input
            Console.SetOut(originalOut);
            Console.SetIn(originalIn);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidUserInput_ReturnsFalse_WhenArgsAreEmpty()
        {
            // Arrange
            string hotelFile = null;
            string bookingFile = null;
            string[] args = new string[0];

            // Suppress Console output and input to prevent blocking on Console.ReadLine()
            var originalOut = Console.Out;
            var originalIn = Console.In;
            using var writer = new StringWriter();
            using var reader = new StringReader(string.Empty);  // Mock an empty line for ReadLine
            Console.SetOut(writer);
            Console.SetIn(reader);

            // Act
            bool result = Common.IsValidUserInput(args, ref hotelFile, ref bookingFile);

            // Reset the console output and input
            Console.SetOut(originalOut);
            Console.SetIn(originalIn);

            // Assert
            Assert.False(result);
        }

        // Test when file extension is not .json
        [Fact]
        public void IsValidFileAndData_ReturnsFalse_WhenFileExtensionIsNotJson()
        {
            // Arrange
            string hotelFile = "hotel.txt";  // Invalid file extension
            var originalOut = Console.Out;
            var originalIn = Console.In;
            using var writer = new StringWriter();
            using var reader = new StringReader(string.Empty);  // Mocking ReadLine to prevent blocking
            Console.SetOut(writer);
            Console.SetIn(reader); // Suppress Console input and output

            // Act
            bool result = Common.IsValidFileAndData(hotelFile);

            // Reset Console
            Console.SetOut(originalOut);
            Console.SetIn(originalIn);

            // Assert
            Assert.False(result);
            Assert.Contains("hotel.txt is an incorrect file type.", writer.ToString());
        }

        // Test when file is not found
        [Fact]
        public void IsValidFileAndData_ReturnsFalse_WhenFileNotFound()
        {
            // Arrange
            string hotelFile = "nonexistentfile.json"; // File does not exist
            var originalOut = Console.Out;
            var originalIn = Console.In;
            using var writer = new StringWriter();
            using var reader = new StringReader(string.Empty);  // Mocking ReadLine to prevent blocking
            Console.SetOut(writer);
            Console.SetIn(reader); // Suppress Console input and output

            // Act
            bool result = Common.IsValidFileAndData(hotelFile);

            // Reset Console
            Console.SetOut(originalOut);
            Console.SetIn(originalIn);

            // Assert
            Assert.False(result);
            Assert.Contains("nonexistentfile.json do not contain valid JSON data.", writer.ToString());
        }

        // Test when file content is invalid JSON
        [Fact]
        public void IsValidFileAndData_ReturnsFalse_WhenFileContentIsInvalidJson()
        {
            // Arrange
            string hotelFile = "invalidjson.json"; // Assume this file contains invalid JSON
            var originalOut = Console.Out;
            var originalIn = Console.In;
            using var writer = new StringWriter();
            using var reader = new StringReader(string.Empty);  // Mocking ReadLine to prevent blocking
            Console.SetOut(writer);
            Console.SetIn(reader); // Suppress Console input and output

            // Act
            bool result = Common.IsValidFileAndData(hotelFile);

            // Reset Console
            Console.SetOut(originalOut);
            Console.SetIn(originalIn);

            // Assert
            Assert.False(result);
            Assert.Contains("invalidjson.json do not contain valid JSON data.", writer.ToString());
        }

        // Test when file is valid JSON
        [Fact]
        public void IsValidFileAndData_ReturnsTrue_WhenFileIsValidJson()
        {
            // Arrange
            string hotelFile = "hotels.json"; // Assume this file contains valid JSON
            var originalOut = Console.Out;
            var originalIn = Console.In;
            using var writer = new StringWriter();
            using var reader = new StringReader(string.Empty);  // Mocking ReadLine to prevent blocking
            Console.SetOut(writer);
            Console.SetIn(reader); // Suppress Console input and output

            // Act
            bool result = Common.IsValidFileAndData(hotelFile);

            // Reset Console
            Console.SetOut(originalOut);
            Console.SetIn(originalIn);

            // Assert
            Assert.True(result);
        }
    }
}
