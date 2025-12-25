using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using DomainEntity.Entities.Enums;

namespace DomainEntity.Entities.Models;
public class Batch
{
    public Guid Id { get; set; }
    public int? Number { get; set; }
    public BatchStatus Status { get; set; }
    public DateTime? Arrival { get; set; }
    public DateTime? Departure { get; set; }
    public DateTime? Expiration { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ProductStatus ProductStatus { get; set; }

    [ForeignKey(nameof(Product))]
    public Guid ProductId { get; set; }

    [JsonIgnore]
    public virtual Product Product { get; set; }

    [ForeignKey(nameof(Storage))]
    public Guid StorageId { get; set; }

    [JsonIgnore]
    public virtual Storage Storage { get; set; }
}
