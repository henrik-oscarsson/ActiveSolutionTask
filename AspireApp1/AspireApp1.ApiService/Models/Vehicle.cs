using System.Text.Json.Serialization;

namespace AspireApp1.ApiService.Models;

[System.ComponentModel.DataAnnotations.Schema.Table(TableName)]
public class Vehicle
{
    public const string TableName = nameof(Vehicle);
    
    public int Id { get; set; }
    public required string RegistrationNumber { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required VehicleCategory Category { get; set; }
    public required int MeterSetting { get; set; }
}