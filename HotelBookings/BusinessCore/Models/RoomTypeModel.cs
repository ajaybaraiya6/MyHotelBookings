namespace HotelBookings.BusinessCore.Models
{
    public class RoomTypeModel
    {
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Amenities { get; set; } = new();
        public List<string> Features { get; set; } = new();
    }
}
