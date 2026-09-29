namespace HomeAwayFromHome.DTOs
{
    public class PropertyResponse
    {
        public int PropertyID { get; set; }
        public string PropertyName { get; set; } = "";
        public string Description { get; set; } = "";
        public string Address { get; set; } = "";
        public int MaximumGuests { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public decimal PricePerNight { get; set; }
        public List<string> Amenities { get; set; } = new();
    }
}