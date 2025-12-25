using System.ComponentModel.DataAnnotations;
using DomainEntity.Entities.Enums;

namespace DomainEntity.Entities.Dto;
public class BatchUpdateDto
{
    public Guid ProductId { get; set; }
    public Guid StorageId { get; set; }
    public BatchStatus Status { get; set; }
    public ProductStatus ProductStatus { get; set; }

    public int? Number { get; set; }

    public DateTime? Arrival { get; set; }
    public DateTime? Departure { get; set; }
    public DateTime? Expiration { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
