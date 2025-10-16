using AspireApp1.ApiService.Models;

namespace AspireApp1.ApiService.Services;

public interface ICustomerService
{
    Task<List<Customer>> GetAllCustomers();
    Task SeedCustomers();
}