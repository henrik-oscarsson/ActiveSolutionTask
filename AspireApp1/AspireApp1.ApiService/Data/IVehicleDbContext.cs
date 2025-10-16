using AspireApp1.ApiService.Models;

namespace AspireApp1.ApiService.Data;

public interface IVehicleDbContext
{
    Task SeedVehicles();
    Task<List<Vehicle>> GetAllVehicles();
    Task<Vehicle?> GetVehicle(int vehicleId);
    Task AddVehicle(Vehicle vehicle);
    Task Return(int bookingVehicleId, int meterSetting);
}