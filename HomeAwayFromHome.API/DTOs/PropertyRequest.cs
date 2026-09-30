using System.ComponentModel.DataAnnotations;

namespace HomeAwayFromHome.API.DTOs
{
    public class PropertyRequest
    {
        [Required, StringLength(100)]
        public string PropertyName { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required, StringLength(250)]
        public string Address { get; set; } = string.Empty;

        [Range(1, 50)]
        public int MaximumGuests { get; set; }

        [Range(1, 20)]
        public int Bedrooms { get; set; }

        [Range(1, 20)]
        public int Bathrooms { get; set; }

        [Range(typeof(decimal), "0.01", "100000")]
        public decimal PricePerNight { get; set; }
    }
}