using AspireApp1.ApiService.Data;
using AspireApp1.ApiService.Models;

namespace AspireApp1.ApiService.Services;

public class VehicleService(
    IVehicleDbContext dbContext,
    ILogger<VehicleService> logger) : IVehicleService
{
    public async Task AddVehicle(Vehicle vehicle)
    {
        try
        {
            await dbContext.AddVehicle(vehicle);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Add vehicle failed");
            throw;
        }
    }
    
    public Task<List<Vehicle>> GetAllVehicles()
    {
        return dbContext.GetAllVehicles();
    }

    public Task SeedVehicles()
    {
        return dbContext.SeedVehicles();
    }
}