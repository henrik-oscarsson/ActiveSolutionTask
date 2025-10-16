using System.Reflection;
using Asp.Versioning;
using AspireApp1.ApiService;
using AspireApp1.ApiService.Data;
using AspireApp1.ApiService.Services;
using Dapper;
using SqlConnection = Microsoft.Data.SqlClient.SqlConnection;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", false)
    .Build();

var connStr = config.GetConnectionString("SqlConnection");
var builder = WebApplication.CreateBuilder(args);
var loggerFactory = LoggerFactory.Create(x => x.AddConfiguration(config));

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Wide open for simple demo project
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy => { policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });
});

builder.Services.AddControllers();
// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiVersioning(versionConfig => {
    versionConfig.DefaultApiVersion = new ApiVersion(1, 0);
    versionConfig.AssumeDefaultVersionWhenUnspecified = true;
});
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Dependency Injection
builder.Services.AddSingleton<ISqlConnectionProvider, SqlConnectionProvider>();
builder.Services.AddSingleton<IVehicleDbContext, VehicleDbContext>();
builder.Services.AddSingleton<ICustomerDbContext, CustomerDbContext>();
builder.Services.AddSingleton<IBookingDbContext, BookingDbContext>();
builder.Services.AddSingleton<ISettingsDbContext, SettingsDbContext>();
builder.Services.AddSingleton<IVehicleService, VehicleService>();
builder.Services.AddSingleton<ICustomerService, CustomerService>();
builder.Services.AddSingleton<IBookingService, BookingService>();
builder.Services.AddSingleton<ISettingsService, SettingsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.UseCors();

app.UseHttpsRedirection();

app.MapControllers();
app.MapHealthChecks("/health");
app.Use(async (context, next) =>
{
    Console.WriteLine($"Endpoint: {context.GetEndpoint()?.DisplayName ?? "(null)"}");
    await next(context);
});

try
{
    await using (app)
    {
        await ConfigureDatabase(config);
        // Add some initial data to the database
        await app.Services.GetRequiredService<IVehicleService>().SeedVehicles();
        await app.Services.GetRequiredService<ICustomerService>().SeedCustomers();
        await app.Services.GetRequiredService<ISettingsService>().SeedSettings();
        await app.RunAsync();
    }
}
catch (Exception e)
{
    Console.WriteLine(e);
}

app.MapDefaultEndpoints();

app.Run();


static async Task ConfigureDatabase(IConfiguration configuration)
{
    var connStr = configuration[Constants.Configuration.ConnectionString]!;
    string[] parts = connStr.Split(";", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
    var genericConnStr = string.Join(";", parts.Where(x => !x.StartsWith("Initial Catalog")));

    await using var connection = new SqlConnection(genericConnStr);
    try
    {
        await connection.OpenAsync();
    }
    catch (Exception e)
    {
        throw new InvalidOperationException("Failed to connect to database", e);
    }
    var assembly = typeof(Program).Assembly;

    try
    {
        var createDatabaseScriptFilePath = assembly.GetManifestResourceNames()
            .FirstOrDefault(x => x.Contains("CreateDatabase.sql"));
        if (string.IsNullOrWhiteSpace(createDatabaseScriptFilePath))
        {
            throw new ArgumentException($"Embedded SQL Script 'CreateDatabase.sql' not found");
        }
        await ExecuteSqlScript(assembly, connection, createDatabaseScriptFilePath);
    }
    catch (Exception e)
    {
        throw new InvalidOperationException("Failed to create database", e);
    }

    try
    {
        var resetDatabaseScriptFilePath = assembly.GetManifestResourceNames()
            .FirstOrDefault(x => x.Contains("ResetDatabase.sql"));
        if (string.IsNullOrWhiteSpace(resetDatabaseScriptFilePath))
        {
            throw new ArgumentException($"Embedded SQL Script 'ResetDatabase.sql' not found");
        }
        await ExecuteSqlScript(assembly, connection, resetDatabaseScriptFilePath);
    }
    catch (Exception e)
    {
        throw new InvalidOperationException("Failed to reset database", e);
    }

    try
    {
        var initializeDatabaseScriptFilePath = assembly.GetManifestResourceNames()
            .FirstOrDefault(x => x.Contains("InitializeDatabase.sql"));
        if (string.IsNullOrWhiteSpace(initializeDatabaseScriptFilePath))
        {
            throw new ArgumentException($"Embedded SQL Script 'InitializeDatabase.sql' not found");
        }
        await ExecuteSqlScript(assembly, connection, initializeDatabaseScriptFilePath);
    }
    catch (Exception e)
    {
        throw new InvalidOperationException("Failed to initialize database", e);
    }

    static async Task ExecuteSqlScript(Assembly assembly, SqlConnection connection, string scriptPath)
    {
        string sqlScriptString;
        await using (var stream = assembly.GetManifestResourceStream(scriptPath))
        using (var streamReader = new StreamReader(stream!))
        {
            sqlScriptString = await streamReader.ReadToEndAsync();
        }

        List<string> sqlCommands = sqlScriptString.Split("GO", StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Replace(@"\n", Environment.NewLine))
            .Where(x => x.Length > 3)
            .ToList();
        int i = 0;
        foreach (string sqlCommand in sqlCommands)
        {
            System.Diagnostics.Debug.WriteLine($"Execute sql command {i}:");
            System.Diagnostics.Debug.WriteLine(sqlCommand);
            i++;
            await connection.ExecuteAsync(sqlCommand);
        }
    }
}