using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DomainEntity.Entities.Models;
public class Measurement
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public double? Temperature { get; set; }
    public double? Humidity { get; set; }
    public bool? Light { get; set; }

    [ForeignKey(nameof(Sensor))]
    public Guid SensorId { get; set; }

    [JsonIgnore]
    public Sensor Sensor { get; set; }
}
