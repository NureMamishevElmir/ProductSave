using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DomainEntity.Entities.Models;
public class Storage
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; }

    public string Address { get; set; }

    public int Workers { get; set; }

    public DateTime CrearedAt { get; private set; } = DateTime.UtcNow;

    [JsonIgnore]
    public ICollection<User> Users { get; set; } = new Collection<User>();

    [JsonIgnore]
    public ICollection<Batch> Batches { get; set; } = new Collection<Batch>();

    [JsonIgnore]
    public ICollection<Sensor> Sensors { get; set; } = new Collection<Sensor>();

}
