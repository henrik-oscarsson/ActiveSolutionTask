using AspireApp1.ApiService.Models;
using AspireApp1.ApiService.Services;
using Dapper;
using Dapper.Contrib.Extensions;

namespace AspireApp1.ApiService.Data;

public class SettingsDbContext(
    ISqlConnectionProvider sqlConnectionProvider,
        ILogger<SettingsDbContext> logger) : ISettingsDbContext
    {
        public async Task<Setting> GetSettings()
        {
            try
            {
                await using var connection = sqlConnectionProvider.Create();
                var settings = await connection.QueryFirstOrDefaultAsync<Setting>($"select * from {Setting.TableName}");
                return settings ?? new Setting();
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Failed to get settings");
            }
            return new Setting();
        }

        public async Task SeedSettings()
        {
            try
            {
                await using var connection = sqlConnectionProvider.Create();
                var setting = await connection.QueryFirstOrDefaultAsync<Setting>($"select * from {Setting.TableName}");
                if (setting == null)
                {
                    setting = Setting.CreateDefault();
                    await connection.InsertAsync(setting);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Failed to get settings");
            }

        }
    }