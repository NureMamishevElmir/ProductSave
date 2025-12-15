using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using DomainEntity.Entities.Enums;

namespace DomainEntity.Entities.Models;
public class Sensor
{
    public Guid Id { get; set; }
    public SensorType Type { get; set; }

    [ForeignKey(nameof(Storage))]
    public Guid StorageId { get; set; }

    [JsonIgnore]
    public virtual Storage? Storage { get; set; }

    [JsonIgnore]
    public ICollection<Measurement> Measurements { get; set; } = new Collection<Measurement>();
    public DateTime LastUpdate { get; set; } = DateTime.UtcNow;
}
