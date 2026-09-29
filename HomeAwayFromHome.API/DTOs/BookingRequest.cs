using System.ComponentModel.DataAnnotations;

namespace HomeAwayFromHome.API.DTOs
{
    public class BookingRequest
    {
        [Range(1, int.MaxValue)]
        public int PropertyID { get; set; }

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        [Range(1, 50)]
        public int NumberOfGuests { get; set; }
    }
}