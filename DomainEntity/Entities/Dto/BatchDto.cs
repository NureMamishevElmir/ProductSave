using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using DomainEntity.Entities.Enums;
using DomainEntity.Entities.Models;

namespace DomainEntity.Entities.Dto;
public class BatchDto
{
    [ForeignKey(nameof(Product))]
    public Guid ProductId { get; set; }

    public DateTime Arrival { get; set; }

    [JsonIgnore]
    public virtual Product? Product { get; set; }

    [ForeignKey(nameof(Storage))]
    public Guid StorageId { get; set; }

    [JsonIgnore]
    public virtual Storage? Storage { get; set; }

    public int? Number { get; set; }

    public BatchStatus Status { get; set; }
    public ProductStatus ProductStatus { get; set; }
}
