namespace DomainEntity.Entities.Dto;
public class MeasurementDto
{
    public Guid SensorId { get; set; }
    public double? Temperature { get; set; }
    public double? Humidity { get; set; }
    public bool? Light { get; set; }
    public DateTime? Timestamp { get; set; }
}
