using HotelBookings.BusinessCore.Models;
using HotelBookings.BusinessCore.Services;

namespace HotelBookings.Tests
{
    public class BookingServiceTests
    {
        private BookingServices service = new BookingServices();

        private List<HotelModel> GetTestHotels() =>
            new()
            {
            new HotelModel
            {
                Id = "H1",
                Rooms = new List<RoomModel>
                {
                    new RoomModel { RoomId = "101", RoomType = "SGL" },
                    new RoomModel { RoomId = "102", RoomType = "SGL" },
                    new RoomModel { RoomId = "201", RoomType = "DBL" }
                }
            }
            };

        private List<BookingModel> GetTestBookings() =>
            new()
            {
            new BookingModel
            {
                HotelId = "H1",
                RoomType = "SGL",
                Arrival = DateTime.Today.AddDays(1).ToString("yyyyMMdd"),
                Departure = DateTime.Today.AddDays(3).ToString("yyyyMMdd")
            },
            new BookingModel
            {
                HotelId = "H1",
                RoomType = "DBL",
                Arrival = DateTime.Today.AddDays(2).ToString("yyyyMMdd"),
                Departure = DateTime.Today.AddDays(4).ToString("yyyyMMdd")
            }
            };

        // AVAILABILITY TESTS
        [Theory]
        [InlineData("Availability(H1, 20250401-20250405, SGL)", "Total available rooms count: 2")]
        [InlineData("Availability(H1, 20250401-20250405, DBL)", "Total available rooms count: 1")]
        [InlineData("Availability(H1, 20250401-20250405, TRP)", "No such a room TRP exist in hotel: H1")]
        [InlineData("Availability(H2, 20250401-20250405, SGL)", "No Hotel found for hotel id: H2")]
        [InlineData("InvalidCommand()", "Unable to read the commmand")]
        public void Availability_ShouldHandleVariousCases(string command, string expected)
        {
            var hotels = GetTestHotels();
            var bookings = GetTestBookings();

            var result = service.Availability(command, hotels, bookings);

            Assert.Contains(expected, result);
        }

        // SEARCH TESTS
        [Theory]
        [InlineData("Search(H2, 3, SGL)", "No Hotel found for hotel id: H2")]
        [InlineData("Search(H1, 3, TRP)", "No such a room TRP exist in hotel: H1")]
        [InlineData("Search(H1, BADVALUE, SGL)", "Unable to read the commmand! Try again.")]
        [InlineData("SearchThisCommandIsBad()", "Unable to read the commmand! Try again.")]
        public void Search_ShouldHandleVariousCases(string command, string expected)
        {
            var hotels = GetTestHotels();
            var bookings = GetTestBookings();

            var result = service.Search(command, hotels, bookings);

            Assert.Contains(expected, result);
        }

        // Optional: Additional test for validating dynamic date blocks (if desired)
        [Fact]
        public void Search_ShouldReturnCorrectAvailabilityBlocks()
        {
            var hotels = GetTestHotels();
            var bookings = GetTestBookings();

            string command = $"Search(H1, 5, SGL)";
            string result = service.Search(command, hotels, bookings);

            // Example: Should return blocks like (YYYYMMDD-YYYYMMDD, 2) etc.
            Assert.Matches(@"\(\d{8}-\d{8}, \d+\)", result);
        }
    }
}
