using AspireApp1.ApiService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AspireApp1.ApiService.Controllers;

public class CustomersController(ICustomerService customerService, ILogger<CustomersController> logger) : V1ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var customers = await customerService.GetAllCustomers();
        return Ok(customers);
    }
}