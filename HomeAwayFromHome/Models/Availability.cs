using System.ComponentModel.DataAnnotations;

namespace HomeAwayFromHome.Models
{
    public class Availability
    {
        [Key]
        public int AvailabilityID { get; set; }

        public int PropertyID { get; set; }

        public Property? Property { get; set; }

        [Required]
        public DateTime AvailableFrom { get; set; }

        [Required]
        public DateTime AvailableTo { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Available";
    }
}
