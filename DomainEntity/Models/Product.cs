namespace DomainEntity.Models;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public double Temperature { get; set; }
    public double Humidity { get; set; }
    public int Number { get; set; }
    public DateTime UpdatedAt { get; set; }
}
