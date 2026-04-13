using BtsBlancApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BtsBlancApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _db;

    public AdminController(AppDbContext db) => _db = db;

    [HttpPost("promouvoir/{userId}")]
    public async Task<IActionResult> Promouvoir(int userId)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user is null) return NotFound("Utilisateur introuvable.");
        if (user.Role == "Admin") return BadRequest("L'utilisateur est déjà admin.");

        user.Role = "Admin";
        await _db.SaveChangesAsync();
        return Ok($"{user.Username} est maintenant admin.");
    }

    [HttpPost("depromouvoir/{userId}")]
    public async Task<IActionResult> Depromouvoir(int userId)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user is null) return NotFound("Utilisateur introuvable.");
        if (user.Role == "User") return BadRequest("L'utilisateur est déjà un user.");

        user.Role = "User";
        await _db.SaveChangesAsync();
        return Ok($"{user.Username} est maintenant un user.");
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers() =>
        Ok(await _db.Users.Select(u => new { u.Id, u.Username, u.Email, u.Role }).ToListAsync());
}
