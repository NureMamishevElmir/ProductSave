using System.ComponentModel.DataAnnotations;

namespace DomainEntity.Entities.Models;
public class Worker
{
    public Guid Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(100)]
    public string Surname { get; set; }

    [MaxLength(20)]
    public string Phone { get; set; }

}
