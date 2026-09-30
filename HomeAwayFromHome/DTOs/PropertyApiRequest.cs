using System.ComponentModel.DataAnnotations;

namespace HomeAwayFromHome.DTOs
{
    public class PropertyApiRequest
    {
        [Required]
        [StringLength(100)]
        public string PropertyName { get; set; } = "";

        [Required]
        public string Description { get; set; } = "";

        [Required]
        [StringLength(250)]
        public string Address { get; set; } = "";

        [Range(1, 50)]
        public int MaximumGuests { get; set; }

        [Range(1, 20)]
        public int Bedrooms { get; set; }

        [Range(1, 20)]
        public int Bathrooms { get; set; }

        [Range(0, 100000)]
        public decimal PricePerNight { get; set; }
    }
}