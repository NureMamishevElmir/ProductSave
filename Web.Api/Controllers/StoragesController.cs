using DomainEntity.Entities.Dto;
using DomainEntity.Entities.Models;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var entity = await _db.Storages
            .Include(s => s.Batches)
            .Include(s => s.Users)
            .Include(s => s.Sensors)
            .FirstOrDefaultAsync(s => s.Id == id);

        return entity == null ? NotFound() : Ok(entity);
    }

    [Authorize(Roles = "Admin")]
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

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
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

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
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
