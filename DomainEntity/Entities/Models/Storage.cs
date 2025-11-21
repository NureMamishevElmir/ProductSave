using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DomainEntity.Entities.Models;
public class Storage
{
    public Guid Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(100)]
    public string Address { get; set; }

    [MaxLength(3)]
    public double Temperature { get; set; }

    [MaxLength(3)]
    public double Humidity { get; set; }

    [MaxLength(10)]
    public int Worker { get; set; }

    [ForeignKey(nameof(Manager))]
    public Guid ManagerId { get; set; }

    [JsonIgnore]
    public virtual Manager Manager { get; set; }

    [JsonIgnore]
    public ICollection<Batch> Batches { get; set; } = new Collection<Batch>();

    [JsonIgnore]
    public ICollection<Worker> Workers { get; set; } = new Collection<Worker>();
}
