using AspireApp1.ApiService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AspireApp1.ApiService.Controllers;

public class VehiclesController(IVehicleService vehicleService, ILogger<VehiclesController> logger) : V1ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var vehicles = await vehicleService.GetAllVehicles();
        return Ok(vehicles);
    }
}