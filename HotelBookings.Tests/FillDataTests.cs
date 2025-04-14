using HotelBookings.BusinessCore.Models;
using HotelBookings.BusinessCore;
namespace HotelBookings.Tests
{
    public class FillDataTests
    {
        [Fact]
        public void FillDataListFromFile_ShouldPopulateHotelsAndBookings_WhenValidJsonFilesProvided()
        {
            // Arrange
            string hotelJson = "[{\r\n\t\t\"id\": \"H1\",\r\n\t\t\"name\": \"Hotel California\",\r\n\t\t\"roomTypes\": [{\r\n\t\t\t\t\"code\": \"SGL\",\r\n\t\t\t\t\"description\": \"Single Room\",\r\n\t\t\t\t\"amenities\": [\"WiFi\", \"TV\"],\r\n\t\t\t\t\"features\": [\"Non-smoking\"]\r\n\t\t\t}, {\r\n\t\t\t\t\"code\": \"DBL\",\r\n\t\t\t\t\"description\": \"Double Room\",\r\n\t\t\t\t\"amenities\": [\"WiFi\", \"TV\", \"Minibar\"],\r\n\t\t\t\t\"features\": [\"Non-smoking\", \"Sea View\"]\r\n\t\t\t}\r\n\t\t],\r\n\t\t\"rooms\": [{\r\n\t\t\t\t\"roomType\": \"SGL\",\r\n\t\t\t\t\"roomId\": \"101\"\r\n\t\t\t}, {\r\n\t\t\t\t\"roomType\": \"SGL\",\r\n\t\t\t\t\"roomId\": \"102\"\r\n\t\t\t}, {\r\n\t\t\t\t\"roomType\": \"DBL\",\r\n\t\t\t\t\"roomId\": \"201\"\r\n\t\t\t}, {\r\n\t\t\t\t\"roomType\": \"DBL\",\r\n\t\t\t\t\"roomId\": \"202\"\r\n\t\t\t}\r\n\t\t]\r\n\t}\r\n]";
            string bookingJson = "[{\r\n\t\t\"hotelId\": \"H1\",\r\n\t\t\"arrival\": \"20240901\",\r\n\t\t\"departure\": \"20240903\",\r\n\t\t\"roomType\": \"DBL\",\r\n\t\t\"roomRate\": \"Prepaid\"\r\n\t}, {\r\n\t\t\"hotelId\": \"H1\",\r\n\t\t\"arrival\": \"20240902\",\r\n\t\t\"departure\": \"20240905\",\r\n\t\t\"roomType\": \"SGL\",\r\n\t\t\"roomRate\": \"Standard\"\r\n\t}\r\n]";

            string hotelFilePath = Path.GetTempFileName();
            string bookingFilePath = Path.GetTempFileName();

            File.WriteAllText(hotelFilePath, hotelJson);
            File.WriteAllText(bookingFilePath, bookingJson);

            List<HotelModel> hotels = null;
            List<BookingModel> bookings = null;

            // Act
            Common.FillDataListFromFile(hotelFilePath, bookingFilePath, ref hotels, ref bookings);

            // Assert
            Assert.NotNull(hotels);
            Assert.Equal("Hotel California", hotels[0].Name);

            Assert.NotNull(bookings);
            Assert.Equal("20240901", bookings[0].Arrival);

            // Cleanup
            File.Delete(hotelFilePath);
            File.Delete(bookingFilePath);
        }
    }
}