using AspireApp1.ApiService.Models;

namespace AspireApp1.ApiService.Data;

public interface IBookingDbContext
{
    Task<List<BookingInfo>> GetAllBookings();

    Task<Booking?> GetBooking(int bookingId);
    Task<bool> VehicleCanBeBooked(int vehicleId, DateTime pickupDate, DateTime returnDate);

    Task<List<Vehicle>> GetAvailableVehiclesForDateRange(DateTime pickupDate, DateTime returnDate);
    Task AddBooking(Booking booking);
    
    Task RemoveBooking(Booking booking);
    Task Pickup(int bookingId, int meterSetting);
    Task Return(int bookingId, int meterSetting);
}