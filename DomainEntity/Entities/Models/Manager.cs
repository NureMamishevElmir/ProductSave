using System.ComponentModel.DataAnnotations;

namespace DomainEntity.Entities.Models;
public class Manager
{
    public Guid Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(100)]
    public string Sername { get; set; }

    [MaxLength(20)]
    public string PhoneNumber { get; set; }
}
