using AspireApp1.ApiService.Models;
using AspireApp1.ApiService.Services;

namespace AspireApp1.ApiService.Data;

using Dapper.Contrib.Extensions;

public class CustomerDbContext(
    ISqlConnectionProvider sqlConnectionProvider,
    ILogger<CustomerDbContext> logger) : ICustomerDbContext
{
    public async Task SeedCustomers()
    {
        try
        {
            await using var connection = sqlConnectionProvider.Create();
            var existingCustomers = await connection.GetAllAsync<Customer>();
            if (existingCustomers.Any())
                return;
            
            var customer1 = new Customer
            {
                SSN = "12345",
                FirstName = "John",
                LastName = "Doe",
            };
            await connection.InsertAsync(customer1);
            var customer2 = new Customer
            {
                SSN = "56789",
                FirstName = "Jane",
                LastName = "Doe",
            };
            await connection.InsertAsync(customer2);
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Failed to seed Customers");
        }
    }
    
    public async Task<List<Customer>> GetAllCustomers()
    {
        try
        {
            await using var connection = sqlConnectionProvider.Create();
            var customers = await connection.GetAllAsync<Customer>();
            return customers.ToList();
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Failed to get all Customers");
        }
        return [];
    }
}