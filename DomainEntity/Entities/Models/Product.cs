using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DomainEntity.Entities.Enums;

namespace DomainEntity.Entities.Models;

public class Product
{
    public Guid Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(3)]
    public double Temperature { get; set; }

    [MaxLength(3)]
    public double Humidity { get; set; }

    [MaxLength(20)]
    public int Number { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public ProductStatus Status { get; set; }

    [JsonIgnore]
    public ICollection<Batch> Batches { get; set; } = new Collection<Batch>();
}
