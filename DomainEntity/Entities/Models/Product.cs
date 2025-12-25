using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DomainEntity.Entities.Enums;

namespace DomainEntity.Entities.Models;

public class Product
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public double Temperature { get; set; }

    public double Humidity { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [JsonIgnore]
    public ICollection<Batch> Batches { get; set; } = new Collection<Batch>();
}
