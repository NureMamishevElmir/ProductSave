using DomainEntity.Entities.Dto;
using DomainEntity.Entities.Models;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;

    public UsersController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
        => Ok(await _db.Users.Include(u => u.Storage).ToListAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var entity = await _db.Users
            .Include(u => u.Storage)
            .FirstOrDefaultAsync(u => u.Id == id);

        return entity == null ? NotFound() : Ok(entity);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] UserCreateDto dto)
    {
        var storageExists = await _db.Storages.AnyAsync(s => s.Id == dto.StorageId);
        if (!storageExists)
        {
            return BadRequest("Storage not found");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Surname = dto.Surname,
            Phone = dto.Phone,
            Role = dto.Role,
            StorageId = dto.StorageId
        };

        user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok(new User
        {
            Id = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            Phone = user.Phone,
            Role = user.Role,
            StorageId = user.StorageId
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UserUpdateDto dto)
    {
        var entity = await _db.Users.FindAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        entity.Name = dto.Name;
        entity.Surname = dto.Surname;
        entity.Phone = dto.Phone;
        entity.Role = dto.Role;
        entity.StorageId = dto.StorageId;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var entity = await _db.Users.FindAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        _db.Users.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
