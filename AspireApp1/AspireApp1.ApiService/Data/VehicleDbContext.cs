using AspireApp1.ApiService.Models;

namespace AspireApp1.ApiService.Data;

using Dapper.Contrib.Extensions;

public class VehicleDbContext(
    ISqlConnectionProvider sqlConnectionProvider,
    ILogger<VehicleDbContext> logger) : IVehicleDbContext
{
    public async Task SeedVehicles()
    {
        try
        {
            await using var connection = sqlConnectionProvider.Create();
            var existingVehicles = await connection.GetAllAsync<Vehicle>();
            if (existingVehicles.Any())
                return;
            
            var vehicle1 = new Vehicle
            {
                Category = VehicleCategory.Sedan,
                MeterSetting = 100,
                RegistrationNumber = "ABC123"
            };
            await connection.InsertAsync(vehicle1);
            var vehicle2 = new Vehicle
            {
                Category = VehicleCategory.StationWagon,
                MeterSetting = 200,
                RegistrationNumber = "DEF456"
            };
            await connection.InsertAsync(vehicle2);
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Failed to seed vehicle");
        }
    }
    
    public async Task<List<Vehicle>> GetAllVehicles()
    {
        try
        {
            await using var connection = sqlConnectionProvider.Create();
            var vehicles = await connection.GetAllAsync<Vehicle>();
            return vehicles.ToList();
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Failed to get all vehicles");
        }
        return [];
    }

    public async Task<Vehicle?> GetVehicle(int vehicleId)
    {
        await using var connection = sqlConnectionProvider.Create();
        return await connection.GetAsync<Vehicle>(vehicleId);
    }

    public async Task AddVehicle(Vehicle vehicle)
    {
        try
        {
            await using var connection = sqlConnectionProvider.Create();
            await connection.InsertAsync(vehicle);
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Failed to insert vehicle");
        }
    }

    public async Task Return(int vehicleId, int meterSetting)
    {
        await using var connection = sqlConnectionProvider.Create();
        var vehicle = await connection.GetAsync<Vehicle>(vehicleId);
        if (vehicle != null)
        {
            vehicle.MeterSetting = meterSetting;
            await connection.UpdateAsync(vehicle);
        }
    }
}