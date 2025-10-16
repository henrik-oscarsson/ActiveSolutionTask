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
    public async Task GetAllBookings_CallsBookingContext_AndNotVehicleContext_WhenListIsEmpty()
    {
        var bookingService = CreateService();
        _mockBookingDbContext.Setup(b => b.GetAllBookings()).ReturnsAsync(new List<BookingInfo>());
        var actual = await bookingService.GetAllBookings();
        _mockBookingDbContext.Verify(x => x.GetAllBookings(), Times.Once);
        _mockVehicleDbContext.Verify(x => x.GetAllVehicles(), Times.Once);
        _mockSettingsDbContext.Verify(x => x.GetSettings(), Times.Once);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task GetAllBookings_CalculatesEstimatedCost()
    {
        // Arrange
        var bookingService = CreateService();
        var vehicleId = 5;
        var bookingId = 7;
        _mockBookingDbContext.Setup(x => x.GetAllBookings()).ReturnsAsync(new List<BookingInfo> { new BookingInfo { Id = bookingId, VehicleId = vehicleId, EstimatedCost = 100 } });
        _mockVehicleDbContext.Setup(x => x.GetAllVehicles()).ReturnsAsync(new List<Vehicle> {new Vehicle { Id = vehicleId, Category = VehicleCategory.Sedan, RegistrationNumber = "ABC13", MeterSetting = 500 }});
        _mockSettingsDbContext.Setup(x => x.GetSettings()).ReturnsAsync(Setting.CreateDefault());        
        // Act
        var actual  =await bookingService.GetAllBookings();
        // Assert
        _mockBookingDbContext.Verify(x => x.GetAllBookings(), Times.Once);
        _mockVehicleDbContext.Verify(x => x.GetAllVehicles(), Times.Once);
        _mockSettingsDbContext.Verify(x => x.GetSettings(), Times.Once);
        var booking = actual.First();
        Assert.Equal(booking.VehicleId, vehicleId);
        Assert.Equal(300m, booking.EstimatedCost);
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
    
    [Fact]
    public void Pickup_UpdatesBooking_And_Vehicle()
    {
        // Arrange
        var bookingService = CreateService();
        var vehicleId = 5;
        var bookingId = 7;
        var meterSetting = 500; 
        _mockBookingDbContext.Setup(x => x.GetBooking(It.IsAny<int>())).ReturnsAsync(new Booking { Id = bookingId, VehicleId = vehicleId });
        _mockVehicleDbContext.Setup(x => x.GetVehicle(It.IsAny<int>())).ReturnsAsync(new Vehicle { Id = vehicleId, Category = VehicleCategory.Sedan, RegistrationNumber = "ABC13", MeterSetting = meterSetting });
        // Act
        var cost = bookingService.Pickup(42);
        // Assert
        _mockBookingDbContext.Verify(x => x.GetBooking(It.IsAny<int>()), Times.Once);
        _mockVehicleDbContext.Verify(x => x.GetVehicle(It.IsAny<int>()), Times.Once);
        _mockBookingDbContext.Verify(x => x.Pickup(It.IsAny<int>(), meterSetting), Times.Once);
    }
    
    [Fact]
    public void Pickup_DoesNothing_IfBookingIsNotFound()
    {
        // Arrange
        var bookingService = CreateService();
        _mockBookingDbContext.Setup(x => x.GetBooking(It.IsAny<int>())).ReturnsAsync((Booking?)null);
        // Act
        var cost = bookingService.Pickup(42);
        // Assert
        _mockBookingDbContext.Verify(x => x.GetBooking(It.IsAny<int>()), Times.Once);
        _mockVehicleDbContext.Verify(x => x.GetVehicle(It.IsAny<int>()), Times.Never);
        _mockBookingDbContext.Verify(x => x.Pickup(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void Return_UpdatesBooking_And_Vehicle()
    {
        // Arrange
        var bookingService = CreateService();
        var vehicleId = 5;
        var bookingId = 7;
        var meterSetting = 500; 
        _mockBookingDbContext.Setup(x => x.GetBooking(It.IsAny<int>())).ReturnsAsync(new Booking { Id = bookingId, VehicleId = vehicleId });
        // Act
        var cost = bookingService.Return(42, meterSetting);
        // Assert
        _mockBookingDbContext.Verify(x => x.GetBooking(It.IsAny<int>()), Times.Once);
        _mockBookingDbContext.Verify(x => x.Return(It.IsAny<int>(), meterSetting), Times.Once);
        _mockVehicleDbContext.Verify(x => x.Return(It.IsAny<int>(), meterSetting), Times.Once);
    }
    
    [Fact]
    public void Return_DoesNothing_IfBookingIsNotFound()
    {
        // Arrange
        var bookingService = CreateService();
        _mockBookingDbContext.Setup(x => x.GetBooking(It.IsAny<int>())).ReturnsAsync((Booking?)null);
        // Act
        var cost = bookingService.Return(42, 500);
        // Assert
        _mockBookingDbContext.Verify(x => x.GetBooking(It.IsAny<int>()), Times.Once);
        _mockBookingDbContext.Verify(x => x.Return(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        _mockVehicleDbContext.Verify(x => x.Return(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }
}