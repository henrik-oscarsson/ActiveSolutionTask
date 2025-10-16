using AspireApp1.ApiService.Models;

namespace AspireApp1.ApiService.Services;

public interface ISettingsService
{
    Task<Setting> GetSettings();
    Task SeedSettings();
}