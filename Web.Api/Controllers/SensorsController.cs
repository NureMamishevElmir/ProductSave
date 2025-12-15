using DomainEntity.Entities;
using DomainEntity.Entities.Dto;
using DomainEntity.Entities.Models;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SensorsController : ControllerBase
{
    private readonly AppDbContext _db;

    public SensorsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<Sensor>>> GetAllAsync()
    {
        var list = await _db.Sensors.AsNoTracking().ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id}")]
    [ActionName(nameof(GetByIdAsync))]
    public async Task<ActionResult<Sensor>> GetByIdAsync(Guid id)
    {
        var entity = await _db.Sensors.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
        {
            return NotFound();
        }

        return Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<Sensor>> CreateAsync([FromBody] SensorDto dto)
    {
        var entity = new Sensor
        {
            Id = Guid.NewGuid(),
            Type = dto.Type,
            StorageId = dto.StorageId
        };

        _db.Sensors.Add(entity);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetByIdAsync), new { id = entity.Id }, entity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] SensorDto dto)
    {
        var entity = await _db.Sensors.FindAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        entity.Type = dto.Type;
        entity.StorageId = dto.StorageId;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var entity = await _db.Sensors.FindAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        _db.Sensors.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
