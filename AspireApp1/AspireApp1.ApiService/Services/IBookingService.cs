using AspireApp1.ApiService.Models;

namespace AspireApp1.ApiService.Services;

public interface IBookingService
{
    Task<List<BookingInfo>> GetAllBookings();
    Task<List<Vehicle>> GetAvailableVehiclesForDateRange(DateTime pickupDate, DateTime returnDate);
    Task AddBooking(int vehicleId, int customerId, DateTime pickupDate, DateTime returnDate);

    decimal CalculateCost(VehicleCategory vehicleCategory, decimal baseRent, int numberOfDays,
        decimal baseKilometerPrice, int numberOfKilometers);

    Task Pickup(int bookingId);
    Task Return(int bookingId, int meterSetting);
}