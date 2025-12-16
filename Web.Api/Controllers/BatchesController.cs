using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DomainEntity.Entities.Models;
using DomainEntity.Entities.Dto;
using Microsoft.AspNetCore.Authorization;

namespace Web.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BatchesController : ControllerBase
{
    private readonly AppDbContext _db;

    public BatchesController(AppDbContext db)
    {
        _db = db;
    }

    private IActionResult? ValidateDates(
    DateTime? arrival,
    DateTime? departure,
    DateTime? expiration,
    DateTime? updatedAt = null)
    {
        if (departure.HasValue && departure.Value < arrival)
        {
            return BadRequest("Departure date cannot be earlier than Arrival date.");
        }

        if (expiration.HasValue && expiration.Value < arrival)
        {
            return BadRequest("Expiration date cannot be earlier than Arrival date.");
        }

        if (updatedAt.HasValue && updatedAt.Value < arrival)
        {
            return BadRequest("UpdatedAt cannot be earlier than Arrival date.");
        }

        return null;
    }

    [HttpGet]
    public async Task<ActionResult<List<Batch>>> GetAllAsync()
        => Ok(await _db.Batches
            .AsNoTracking()
            .Include(b => b.Product)
            .ToListAsync());

    [HttpGet("{id}")]
    [ActionName(nameof(GetByIdAsync))]
    public async Task<ActionResult<Batch>> GetByIdAsync(Guid id)
    {
        var entity = await _db.Batches
            .AsNoTracking()
            .Include(b => b.Product)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (entity == null)
        {
            return NotFound();
        }

        return Ok(entity);
    }

    [Authorize(Roles = "Worker")]
    [HttpPost]
    public async Task<ActionResult<Batch>> CreateAsync([FromBody] BatchDto dto)
    {
        if (!await _db.Products.AnyAsync(p => p.Id == dto.ProductId))
        {
            return BadRequest("ProductId not found.");
        }

        var entity = new Batch
        {
            Id = Guid.NewGuid(),
            ProductId = dto.ProductId,
            StorageId = dto.StorageId,

            Status = dto.Status,
            ProductStatus = dto.ProductStatus,
            Number = dto.Number,
        };

        _db.Batches.Add(entity);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetByIdAsync), new { id = entity.Id }, entity);
    }

    [Authorize(Roles = "Worker")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] BatchUpdateDto dto)
    {
        var entity = await _db.Batches.FindAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        var updatedAt = DateTime.UtcNow;

        var validationError = ValidateDates(
            dto.Arrival,
            dto.Departure,
            dto.Expiration,
            updatedAt
        );

        if (validationError != null)
        {
            return validationError;
        }

        entity.ProductId = dto.ProductId;
        entity.StorageId = dto.StorageId;

        entity.Status = dto.Status;
        entity.ProductStatus = dto.ProductStatus;

        entity.Arrival = dto.Arrival;
        entity.Departure = dto.Departure;
        entity.Expiration = dto.Expiration;

        entity.UpdatedAt = updatedAt;
        entity.Number = dto.Number;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var entity = await _db.Batches.FindAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        _db.Batches.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
