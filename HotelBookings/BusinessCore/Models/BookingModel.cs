
namespace HotelBookings.BusinessCore.Models
{
    public class BookingModel
    {
        public string HotelId { get; set; } = string.Empty;
        public string Arrival { get; set; } = string.Empty;
        public string Departure { get; set; } = string.Empty;
        public string RoomType { get; set; } = string.Empty;
        public string RoomRate { get; set; } = string.Empty;
    }
}
