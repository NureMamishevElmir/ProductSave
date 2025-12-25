using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using DomainEntity.Entities.Enums;

namespace DomainEntity.Entities.Models;
public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; }

    public string Surname { get; set; }

    public string Phone { get; set; }

    public string Password { get; set; }

    public UserRole Role { get; set; }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    [ForeignKey(nameof(Storage))]
    public Guid? StorageId { get; set; }

    [JsonIgnore]
    public virtual Storage? Storage { get; set; }
}
