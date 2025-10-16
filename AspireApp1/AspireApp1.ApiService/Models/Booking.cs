namespace AspireApp1.ApiService.Models;

[System.ComponentModel.DataAnnotations.Schema.Table(TableName)]
public class Booking
{
    public const string TableName = nameof(Booking);
    
    public int Id { get; set; }
    
    public DateTime ScheduledPickUpDate { get; set; }
    public DateTime ScheduledReturnDate { get; set; }
    public DateTime? ActualPickUpDate { get; set; }
    public DateTime? ActualReturnDate { get; set; }
    public int VehicleId { get; set; }
    public int CustomerId { get; set; }
    
    public bool IsPickedUp { get; set; }
    public bool IsReturned { get; set; }
    
    public int? PickupMeterSetting { get; set; }
    public int? ReturnMeterSetting { get; set; }
}