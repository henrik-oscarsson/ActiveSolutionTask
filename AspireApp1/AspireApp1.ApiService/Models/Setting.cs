namespace AspireApp1.ApiService.Models;

[System.ComponentModel.DataAnnotations.Schema.Table(TableName)]
public class Setting
{
    public const string TableName = nameof(Setting);

    public decimal BaseRent { get; set; }
    public decimal BaseKilometerPrice { get; set; }
    
    public int DefaultNumberOfDays { get; set; }
    
    public int DefaultNumberOfKilometers { get; set; }

    public static Setting CreateDefault()
    {
        return new Setting
        {
            BaseRent = 100,
            BaseKilometerPrice = 10,
            DefaultNumberOfDays = 3,
            DefaultNumberOfKilometers = 100,
        };
    }
}