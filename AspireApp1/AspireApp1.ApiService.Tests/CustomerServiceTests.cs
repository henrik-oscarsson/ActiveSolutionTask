using AspireApp1.ApiService.Data;
using AspireApp1.ApiService.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace AspireApp1.ApiService.Tests;

public class CustomerServiceTests
{
    private readonly Mock<ICustomerDbContext> _mockDbContext = new ();
    private readonly Mock<ILogger<CustomerService>> _mockLogger = new ();

    [Fact]
    public async Task GetAllCustomers_CallsDbContext()
    {
        var customerService = new CustomerService(_mockDbContext.Object, _mockLogger.Object);
        await customerService.GetAllCustomers();
        _mockDbContext.Verify(x => x.GetAllCustomers(), Times.Once);
    }
}