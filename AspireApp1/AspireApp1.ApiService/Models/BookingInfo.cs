namespace AspireApp1.ApiService.Models;

public class BookingInfo
{
    public int Id { get; set; }
    
    public DateTime ScheduledPickUpDate { get; set; }
    public DateTime ScheduledReturnDate { get; set; }
    public DateTime? ActualPickUpDate { get; set; }
    public DateTime? ActualReturnDate { get; set; }
    public int VehicleId { get; set; }
    public string? VehicleRegistrationNumber { get; set; }
    public int CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public bool IsPickedUp { get; set; }
    public bool IsReturned { get; set; }
    public decimal EstimatedCost { get; set; }
    public decimal ActualCost { get; set; }
    public int? PickupMeterSetting { get; set; }
    public int? ReturnMeterSetting { get; set; }
}