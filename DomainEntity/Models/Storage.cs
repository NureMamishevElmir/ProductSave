using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainEntity.Models;
public class Storage
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public double Temperature { get; set; }
    public double Humidity { get; set; }
    public int StorageWorker { get; set; }
}
