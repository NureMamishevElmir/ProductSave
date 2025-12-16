using DomainEntity.Entities.Dto;
using DomainEntity.Entities.Models;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProductsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<Product>>> GetAllAsync()
        => Ok(await _db.Products.AsNoTracking().ToListAsync());

    [HttpGet("{id}")]
    [ActionName(nameof(GetByIdAsync))]
    public async Task<ActionResult<Product>> GetByIdAsync(Guid id)
    {
        var entity = await _db.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
        {
            return NotFound();
        }

        return Ok(entity);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<Product>> CreateAsync([FromBody] ProductDto dto)
    {
        var entity = new Product
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Temperature = dto.Temperature,
            Humidity = dto.Humidity,
        };

        _db.Products.Add(entity);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetByIdAsync), new { id = entity.Id }, entity);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] ProductDto dto)
    {
        var entity = await _db.Products.FindAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        entity.Name = dto.Name;
        entity.Temperature = dto.Temperature;
        entity.Humidity = dto.Humidity;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var entity = await _db.Products.FindAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        _db.Products.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
