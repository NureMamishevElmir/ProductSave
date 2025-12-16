using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DomainEntity.Entities.Dto;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service;

namespace Web.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly JwtTokenService _jwt;

    public AuthController(AppDbContext db, JwtTokenService jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> LoginAsync([FromBody] LoginDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(x => x.Phone == dto.Phone);
        if (user == null)
        {
            return Unauthorized("Invalid credentials");
        }

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
        {
            return Unauthorized("Invalid credentials");
        }

        var (token, exp) = _jwt.CreateAccessToken(user.Id, user.Role, user.StorageId);

        return Ok(new AuthResponseDto
        {
            AccessToken = token,
            ExpiresAt = exp
        });
    }

    [Authorize]
    [HttpPost("ChangePassword")]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.OldPassword) || string.IsNullOrWhiteSpace(dto.NewPassword))
        {
            return BadRequest("OldPassword and NewPassword are required.");
        }

        if (dto.NewPassword.Length < 8)
        {
            return BadRequest("New password is too short (min 8).");
        }

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (!Guid.TryParse(userIdStr, out var userId))
        {
            return Unauthorized();
        }

        var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (user == null)
        {
            return Unauthorized();
        }

        try
        {
            if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.Password))
            {
                return BadRequest("Old password is incorrect.");
            }
        }
        catch (BCrypt.Net.SaltParseException)
        {
            return BadRequest("Stored password hash is invalid. Reset password.");
        }

        user.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        await _db.SaveChangesAsync();

        return Ok("Password changed.");
    }
}
