namespace AspireApp1.ApiService.Models;

[System.ComponentModel.DataAnnotations.Schema.Table(TableName)]
public class Customer
{
    public const string TableName = nameof(Customer);

    public int Id { get; set; }
    public string SSN { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
