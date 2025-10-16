using AspireApp1.ApiService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AspireApp1.ApiService.Controllers;

public class SettingsController(ISettingsService settingsService, ILogger<SettingsController> logger) : V1ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var settings = await settingsService.GetSettings();
        return Ok(settings);
    }
}