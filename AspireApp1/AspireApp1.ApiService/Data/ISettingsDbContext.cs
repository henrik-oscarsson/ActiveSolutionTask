using AspireApp1.ApiService.Models;

namespace AspireApp1.ApiService.Data;

public interface ISettingsDbContext
{
    Task<Setting> GetSettings();

    Task SeedSettings();
}