namespace AspireApp1.ApiService.Models;

public class CreateBooking
{
    public DateTime PickUpDate { get; set; }
    public DateTime ReturnDate { get; set; }
    public int VehicleId { get; set; }
    public int CustomerId { get; set; }
}
 