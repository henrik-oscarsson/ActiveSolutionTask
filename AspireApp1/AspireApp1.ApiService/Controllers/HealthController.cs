using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AspireApp1.ApiService.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
public class HealthController : BaseController
{
    [HttpGet]
    public IActionResult GetHealthStatus()
    {
        List<HealthCheckResult> healthChecks = [HealthCheckResult.Unhealthy()];
        return Ok(healthChecks);
    }
}