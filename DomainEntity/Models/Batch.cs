using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainEntity.Models;
public class Batch
{
    public Guid Id { get; set; }
    public int Number { get; set; }
    public DateTime Arrival { get; set; }
    public DateTime Departure { get; set; }
    public DateTime Expiration { get; set; }
    public DateTime UpdatedAt { get; set; }
}
