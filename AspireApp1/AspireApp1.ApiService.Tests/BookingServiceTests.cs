using System.ComponentModel.DataAnnotations;
using AspireApp1.ApiService.Data;
using AspireApp1.ApiService.Models;
using AspireApp1.ApiService.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace AspireApp1.ApiService.Tests;

public class BookingServiceTests
{
    private readonly Mock<IBookingDbContext> _mockBookingDbContext = new ();
    private readonly Mock<IVehicleDbContext> _mockVehicleDbContext = new ();
    private readonly Mock<ISettingsDbContext> _mockSettingsDbContext = new ();
    private readonly Mock<ILogger<BookingService>> _mockLogger = new ();

    private IBookingService CreateService()
    {
        return new BookingService(_mockBookingDbContext.Object, _mockVehicleDbContext.Object, _mockSettingsDbContext.Object, _mockLogger.Object);
    }
    [Fact]
    public async Task GetAllBookings_CallsDbContext()
    {
        var bookingService = CreateService();
        await bookingService.GetAllBookings();
        _mockBookingDbContext.Verify(x => x.GetAllBookings(), Times.Once);
    }
    
    [Fact]
    public async Task GetAllAvailableBookings_Throws_IfReturnDateIsBeforeStartDate()
    {
        // Arrange
        var bookingService = CreateService();
        var pickupDate = DateTime.Today;
        var returnDate = DateTime.Today.AddDays(-1);
        // Act
        await Assert.ThrowsAsync<ValidationException>(() => bookingService.GetAvailableVehiclesForDateRange(pickupDate, returnDate));
        // Assert
        _mockBookingDbContext.Verify(x => x.GetAllBookings(), Times.Never);
    }

    [Fact]
    public async Task AddBooking_Throws_IfReturnDateIsBeforeStartDate()
    {
        // Arrange
        var bookingService = CreateService();
        var pickupDate = DateTime.Today;
        var returnDate = DateTime.Today.AddDays(-1);
        // Act
        await Assert.ThrowsAsync<ValidationException>(() => bookingService.AddBooking(1, 2, pickupDate, returnDate));
        // Assert
        _mockBookingDbContext.Verify(x => x.AddBooking(It.IsAny<Booking>()), Times.Never);
    }

    [Fact]
    public async Task AddBooking_IfVehicleIsUnbooked_AddsBookingToDatabase()
    {
        // Arrange
        var bookingService = CreateService();
        _mockBookingDbContext.Setup(x => x.VehicleCanBeBooked(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(true);
        var pickupDate = DateTime.Today;
        var returnDate = DateTime.Today.AddDays(3);
        // Act
        await bookingService.AddBooking(1, 2, pickupDate, returnDate);
        // Assert
        _mockBookingDbContext.Verify(x => x.AddBooking(It.IsAny<Booking>()), Times.Once);
    }

    [Fact]
    public async Task AddBooking_IfVehicleIsBooked_DoesNotAddBookingToDatabase()
    {
        // Arrange
        var bookingService = CreateService();
        _mockBookingDbContext.Setup(x => x.VehicleCanBeBooked(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(false);
        var pickupDate = DateTime.Today;
        var returnDate = DateTime.Today.AddDays(3);
        // Act
        await bookingService.AddBooking(1, 2, pickupDate, returnDate);
        // Assert
        _mockBookingDbContext.Verify(x => x.AddBooking(It.IsAny<Booking>()), Times.Never);
    }

    [Fact]
    public void CalculateCost_Sedan()
    {
        // Arrange
        var bookingService = CreateService();
        // Act
        var cost = bookingService.CalculateCost(VehicleCategory.Sedan, 2m, 5, 10m, 4);
        // Assert
        Assert.Equal(10m, cost);
    }

    [Fact]
    public void CalculateCost_StationWagon()
    {
        // Arrange
        var bookingService = CreateService();
        // Act
        var cost = bookingService.CalculateCost(VehicleCategory.StationWagon, 2m, 5, 10m, 4);
        // Assert
        Assert.Equal(53m, cost);
    }
    
    [Fact]
    public void CalculateCost_Truck()
    {
        // Arrange
        var bookingService = CreateService();
        // Act
        var cost = bookingService.CalculateCost(VehicleCategory.Truck, 2m, 5, 10m, 4);
        // Assert
        Assert.Equal(55m, cost);
    }
}