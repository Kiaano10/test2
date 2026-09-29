using System.ComponentModel.DataAnnotations;

namespace HomeAwayFromHome.API.DTOs
{
    public class ReviewRequest
    {
        [Range(1, int.MaxValue)]
        public int BookingID { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [Required, StringLength(1000)]
        public string Comment { get; set; } = string.Empty;
    }
}