using System.ComponentModel.DataAnnotations;
using AspireApp1.ApiService.Data;
using AspireApp1.ApiService.Models;

namespace AspireApp1.ApiService.Services;

public class BookingService(
    IBookingDbContext bookingDbContext,
    IVehicleDbContext vehicleDbContext,
    ISettingsDbContext settingsDbContext,
    ILogger<BookingService> logger) : IBookingService
{
    public async Task<List<BookingInfo>> GetAllBookings()
    {
        var bookings = await bookingDbContext.GetAllBookings();
        var vehicles = await vehicleDbContext.GetAllVehicles();
        var settings = await settingsDbContext.GetSettings();
        bookings.ForEach(b =>
        {
            var vehicle = vehicles.FirstOrDefault(v => v.Id == b.VehicleId);
            if (vehicle != null)
            {
                b.EstimatedCost = CalculateCost(vehicle.Category, settings.BaseRent, settings.DefaultNumberOfDays, settings.BaseKilometerPrice, settings.DefaultNumberOfKilometers);
                if (b.IsReturned)
                {
                    var numberOfDays = (int)Math.Ceiling((b.ActualReturnDate! - b.ActualPickUpDate!).Value.TotalDays);
                    var numberOfKilometers = (b.ReturnMeterSetting - b.PickupMeterSetting).GetValueOrDefault();
                    b.ActualCost = CalculateCost(vehicle.Category, settings.BaseRent, numberOfDays, settings.BaseKilometerPrice, numberOfKilometers);
                }
            }
        });
        return bookings;
    }
    
    public Task<List<Vehicle>> GetAvailableVehiclesForDateRange(DateTime pickupDate, DateTime returnDate)
    {
        if (returnDate <= pickupDate)
        {
            throw new ValidationException("ReturnDate must be after pickup date");
        }
        return bookingDbContext.GetAvailableVehiclesForDateRange(pickupDate, returnDate);
    }
    
    public async Task AddBooking(int vehicleId, int customerId, DateTime pickupDate, DateTime returnDate)
    {
        if (returnDate <= pickupDate)
        {
            throw new ValidationException("ReturnDate must be after pickup date");
        }

        if (await bookingDbContext.VehicleCanBeBooked(vehicleId, pickupDate, returnDate))
        {
            await bookingDbContext.AddBooking(new Booking { CustomerId = customerId, VehicleId = vehicleId, ScheduledPickUpDate = pickupDate, ScheduledReturnDate = returnDate });
        }
    }
    
    public decimal CalculateCost(VehicleCategory vehicleCategory, decimal baseRent, int numberOfDays, decimal baseKilometerPrice, int numberOfKilometers)
    {
        switch (vehicleCategory)
        {
            case VehicleCategory.Sedan:
                return baseRent * numberOfDays;
            
            case VehicleCategory.StationWagon:
                return (baseRent * numberOfDays * 1.3m) + (baseKilometerPrice * numberOfKilometers);
            
            default:
                return (baseRent * numberOfDays * 1.5m) + (baseKilometerPrice * numberOfKilometers);
        }
    }

    public async Task Pickup(int bookingId)
    {
        var booking = await bookingDbContext.GetBooking(bookingId);
        if (booking != null)
        {
            var vehicle = await vehicleDbContext.GetVehicle(booking.VehicleId);
            if (vehicle != null)
            {
                await bookingDbContext.Pickup(bookingId, vehicle.MeterSetting);
            }
        }
    }

    public async Task Return(int bookingId, int meterSetting)
    {
        var booking = await bookingDbContext.GetBooking(bookingId);
        if (booking != null)
        {
            await vehicleDbContext.Return(booking.VehicleId, meterSetting);
            await bookingDbContext.Return(bookingId, meterSetting);
        }
    }
}