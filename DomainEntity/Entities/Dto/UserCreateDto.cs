using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using DomainEntity.Entities.Enums;
using DomainEntity.Entities.Models;

namespace DomainEntity.Entities.Dto;
public class UserCreateDto
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Phone { get; set; }
    public string Password { get; set; }
    public UserRole Role { get; set; }

    [ForeignKey(nameof(Storage))]
    public Guid? StorageId { get; set; }

    [JsonIgnore]
    public virtual Storage? Storage { get; set; }
}
