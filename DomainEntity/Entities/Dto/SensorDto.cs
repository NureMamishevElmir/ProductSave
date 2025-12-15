using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using DomainEntity.Entities.Enums;
using DomainEntity.Entities.Models;

namespace DomainEntity.Entities.Dto;
public class SensorDto
{
    public SensorType Type { get; set; }

    [ForeignKey(nameof(Storage))]
    public Guid StorageId { get; set; }

    [JsonIgnore]
    public virtual Storage? Storage { get; set; }
}
