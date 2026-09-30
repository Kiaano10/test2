namespace HomeAwayFromHome.DTOs;

public class ReviewApiRequest
{
    public int BookingID { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
}
