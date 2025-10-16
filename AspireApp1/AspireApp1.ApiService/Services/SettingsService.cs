using AspireApp1.ApiService.Data;
using AspireApp1.ApiService.Models;

namespace AspireApp1.ApiService.Services;

public class SettingsService(ISettingsDbContext dbContext) : ISettingsService
{
    public Task<Setting> GetSettings()
    {
        return dbContext.GetSettings();
    }

    public Task SeedSettings()
    {
        return dbContext.SeedSettings();
    }
}