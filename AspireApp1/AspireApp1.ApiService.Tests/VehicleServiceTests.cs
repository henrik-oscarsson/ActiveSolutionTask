using AspireApp1.ApiService.Data;
using AspireApp1.ApiService.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace AspireApp1.ApiService.Tests;

public class VehicleServiceTests
{
    private readonly Mock<IVehicleDbContext> _mockDbContext = new ();
    private readonly Mock<ILogger<VehicleService>> _mockLogger = new ();

    [Fact]
    public async Task GetAllVehicles_CallsDbContext()
    {
        var vehicleService = new VehicleService(_mockDbContext.Object, _mockLogger.Object);
        await vehicleService.GetAllVehicles();
        _mockDbContext.Verify(x => x.GetAllVehicles(), Times.Once);
    }
}