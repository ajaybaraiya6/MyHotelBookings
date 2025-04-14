namespace HotelBookings.BusinessCore.Models
{
    public class HotelModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<RoomTypeModel> RoomTypes { get; set; } = new();
        public List<RoomModel> Rooms { get; set; } = new();
    }
}
