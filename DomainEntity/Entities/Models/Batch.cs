using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DomainEntity.Entities.Models;
public class Batch
{
    public Guid Id { get; set; }

    [MaxLength(100)]
    public int Number { get; set; }
    public DateTime Arrival { get; set; }
    public DateTime? Departure { get; set; }
    public DateTime? Expiration { get; set; }
    public DateTime? UpdatedAt { get; set; }

    [JsonIgnore]
    public ICollection<Product> Products { get; set; } = new Collection<Product>();
}
