using AspireApp1.ApiService.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace AspireApp1.ApiService.Data;

using Dapper.Contrib.Extensions;

public class BookingDbContext(
    ISqlConnectionProvider sqlConnectionProvider,
    ILogger<BookingDbContext> logger) : IBookingDbContext
{
    public async Task<List<BookingInfo>> GetAllBookings()
    {
        try
        {
            await using var connection = sqlConnectionProvider.Create();
            var query = @$"
                SELECT 
                    b.*, 
                    v.RegistrationNumber AS VehicleRegistrationNumber, 
                    CONCAT(c.FirstName, ' ', c.LastName) AS CustomerName
                FROM 
                    {Booking.TableName}
                AS b
                LEFT JOIN 
                {Vehicle.TableName} AS v ON v.Id = b.VehicleId
                LEFT JOIN 
                {Customer.TableName} AS c ON c.Id = b.CustomerId    
                  ";
            var bookings = await connection.QueryAsync<BookingInfo>(query);
            return bookings.ToList();
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Failed to get all bookings");
        }
        return [];
    }
    
    public async Task<List<Vehicle>> GetAvailableVehiclesForDateRange(DateTime pickupDate, DateTime returnDate)
    {
        try
        {
            await using var connection = sqlConnectionProvider.Create();
            var existingBookings = await GetExistingBookings(connection, pickupDate, returnDate);
            if (existingBookings.IsNullOrEmpty())
            {
                var vehicles = await connection.GetAllAsync<Vehicle>();
                return vehicles.ToList();
            }
            var notAvailableCarIds = string.Join(",", existingBookings.Select(x => x.VehicleId));
            var query = $"SELECT * FROM {Vehicle.TableName} as v WHERE v.Id NOT IN ({notAvailableCarIds})";
            var availableVehicles = await connection.QueryAsync<Vehicle>(query);
            return availableVehicles.ToList();
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Failed to get all available vehicles");
        }
        return [];
    }

    public async Task<bool> VehicleCanBeBooked(int vehicleId, DateTime pickupDate, DateTime returnDate)
    {
        await using var connection = sqlConnectionProvider.Create();
        var query = $"SELECT * FROM {Booking.TableName} as b WHERE b.VehicleId = @VehicleId AND b.ScheduledPickUpDate <= @ReturnDate AND b.ScheduledReturnDate >= @PickupDate";
        var parameters = new { VehicleId = vehicleId, PickupDate = pickupDate, ReturnDate = returnDate };
        var existingBookings = await connection.QueryAsync<Booking>(query, parameters);
        return existingBookings.IsNullOrEmpty();
    }

    public async Task<Booking?> GetBooking(int bookingId)
    {
        await using var connection = sqlConnectionProvider.Create();
        return await connection.GetAsync<Booking>(bookingId);
    }
    
    public async Task AddBooking(Booking booking)
    {
        await using var connection = sqlConnectionProvider.Create();
        await connection.InsertAsync(booking);
    }

    public async Task RemoveBooking(Booking booking)
    {
        await using var connection = sqlConnectionProvider.Create();
        await connection.DeleteAsync(booking);
    }

    public async Task Pickup(int bookingId, int meterSetting)
    {
        await using var connection = sqlConnectionProvider.Create();
        var booking = await connection.GetAsync<Booking>(bookingId);
        if (booking != null)
        {
            booking.IsPickedUp = true;
            booking.ActualPickUpDate = DateTime.Now;
            booking.PickupMeterSetting = meterSetting;
            await connection.UpdateAsync(booking);
        }
    }

    public async Task Return(int bookingId, int meterSetting)
    {
        await using var connection = sqlConnectionProvider.Create();
        var booking = await connection.GetAsync<Booking>(bookingId);
        if (booking != null)
        {
            booking.IsReturned = true;
            booking.ActualReturnDate = DateTime.Now;
            booking.ReturnMeterSetting = meterSetting;
            await connection.UpdateAsync(booking);
        }
    }

    public async Task<List<Booking>> GetExistingBookingsForDateRange(DateTime pickupDate, DateTime returnDate)
    {
        await using var connection = sqlConnectionProvider.Create();
        var bookings = await GetExistingBookings(connection, pickupDate, returnDate);
        return bookings != null ? bookings.ToList() : new List<Booking>();
    }
    
    private async Task<List<Booking>?> GetExistingBookings(SqlConnection connection, DateTime pickupDate, DateTime returnDate)
    {
        var query = $"SELECT * FROM {Booking.TableName} as b WHERE b.ScheduledPickupDate <= @ReturnDate AND b.ScheduledReturnDate >= @PickupDate";
        var parameters = new { PickupDate = pickupDate, ReturnDate = returnDate };
        var existingBookings = await connection.QueryAsync<Booking>(query, parameters);
        return existingBookings?.ToList();
    }
}