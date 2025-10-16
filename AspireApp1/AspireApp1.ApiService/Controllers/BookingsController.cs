using AspireApp1.ApiService.Models;
using AspireApp1.ApiService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AspireApp1.ApiService.Controllers;

public class BookingsController(IBookingService bookingService, ILogger<BookingsController> logger) : V1ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var bookings = await bookingService.GetAllBookings();
        return Ok(bookings);
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAllAvailable([FromQuery] DateTime pickupDate, [FromQuery] DateTime returnDate)
    {
        var availableVehicles = await bookingService.GetAvailableVehiclesForDateRange(pickupDate, returnDate);
        return Ok(availableVehicles);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBooking booking)
    {
        try
        {
            await bookingService.AddBooking(booking.VehicleId, booking.CustomerId, booking.PickUpDate, booking.ReturnDate);
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to create booking");
            return BadRequest(e);
        }
    }
    
    [HttpPost("pickup/{bookingId}")]
    public async Task<IActionResult> GetAllAvailable([FromRoute] int bookingId)
    {
        await bookingService.Pickup(bookingId);
        return Ok();
    }
    
    [HttpPost("return/{bookingId}/{meterSetting}")]
    public async Task<IActionResult> GetAllAvailable([FromRoute] int bookingId, [FromRoute] int meterSetting)
    {
        await bookingService.Return(bookingId, meterSetting);
        return Ok();
    }

}