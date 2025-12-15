using DomainEntity.Entities.Dto;
using DomainEntity.Entities.Models;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MeasurementsController : ControllerBase
{
    private readonly AppDbContext _db;

    public MeasurementsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<Measurement>>> GetAllAsync()
    {
        var list = await _db.Measurements.AsNoTracking().ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id:guid}")]
    [ActionName(nameof(GetByIdAsync))]
    public async Task<ActionResult<Measurement>> GetByIdAsync(Guid id)
    {
        var entity = await _db.Measurements.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
        {
            return NotFound();
        }

        return Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<Measurement>> CreateAsync([FromBody] MeasurementDto dto)
    {
        var sensorExists = await _db.Sensors.AnyAsync(x => x.Id == dto.SensorId);
        if (!sensorExists)
        {
            return BadRequest("SensorId not found.");
        }

        var entity = new Measurement
        {
            Id = Guid.NewGuid(),
            SensorId = dto.SensorId,
            Temperature = dto.Temperature,
            Humidity = dto.Humidity,
            Light = dto.Light,
            Timestamp = dto.Timestamp ?? DateTime.UtcNow
        };

        _db.Measurements.Add(entity);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetByIdAsync), new { id = entity.Id }, entity);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] MeasurementDto dto)
    {
        var entity = await _db.Measurements.FindAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        entity.SensorId = dto.SensorId;

        entity.Temperature = dto.Temperature;
        entity.Humidity = dto.Humidity;
        entity.Light = dto.Light;

        entity.Timestamp = dto.Timestamp ?? entity.Timestamp;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var entity = await _db.Measurements.FindAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        _db.Measurements.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
