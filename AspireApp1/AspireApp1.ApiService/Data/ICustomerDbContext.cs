using AspireApp1.ApiService.Models;

namespace AspireApp1.ApiService.Data;

public interface ICustomerDbContext
{
    Task SeedCustomers();
    Task<List<Customer>> GetAllCustomers();
}