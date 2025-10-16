using AspireApp1.ApiService.Models;

namespace AspireApp1.ApiService.Services;

public interface IVehicleService
{
    Task AddVehicle(Vehicle vehicle);
    Task<List<Vehicle>> GetAllVehicles();
    Task SeedVehicles();
}