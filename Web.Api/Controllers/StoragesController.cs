using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DomainEntity.Entities.Models;
using DomainEntity.Entities.Dto;

namespace Web.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StoragesController : ControllerBase
{
    private readonly AppDbContext _db;

    public StoragesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
        => Ok(await _db.Storages
            .Include(s => s.Batches)
            .Include(s => s.Users)
            .Include(s => s.Sensors)
            .ToListAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var entity = await _db.Storages
            .Include(s => s.Batches)
            .Include(s => s.Users)
            .Include(s => s.Sensors)
            .FirstOrDefaultAsync(s => s.Id == id);

        return entity == null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(StorageDto dto)
    {
        var storage = new Storage
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Address = dto.Address
        };

        _db.Storages.Add(storage);
        await _db.SaveChangesAsync();

        return Ok(storage.Id);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, StorageDto dto)
    {
        var entity = await _db.Storages.FindAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        entity.Name = dto.Name;
        entity.Address = dto.Address;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var entity = await _db.Storages.FindAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        _db.Storages.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
