using AspireApp1.ApiService.Data;
using AspireApp1.ApiService.Models;

namespace AspireApp1.ApiService.Services;

public class CustomerService(
    ICustomerDbContext dbContext,
    ILogger<CustomerService> logger) : ICustomerService
{
    public Task<List<Customer>> GetAllCustomers()
    {
        return dbContext.GetAllCustomers();
    }
    
    public Task SeedCustomers()
    {
        return dbContext.SeedCustomers();
    }
}