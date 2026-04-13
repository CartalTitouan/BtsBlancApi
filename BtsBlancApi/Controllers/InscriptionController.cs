using BtsBlancApi.Data;
using BtsBlancApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BtsBlancApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InscriptionController : ControllerBase
{
    private readonly AppDbContext _db;

    public InscriptionController(AppDbContext db) => _db = db;

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost("{evenementId}")]
    public async Task<IActionResult> Inscrire(int evenementId)
    {
        var userId = GetUserId();

        if (!await _db.Evenements.AnyAsync(e => e.Id == evenementId))
            return NotFound("Événement introuvable.");

        if (await _db.Inscriptions.AnyAsync(i => i.UserId == userId && i.EvenementId == evenementId))
            return Conflict("Vous êtes déjà inscrit à cet événement.");

        _db.Inscriptions.Add(new Inscription { UserId = userId, EvenementId = evenementId });
        await _db.SaveChangesAsync();
        return Ok("Inscription réussie.");
    }

    [HttpDelete("{evenementId}")]
    public async Task<IActionResult> Desinscrire(int evenementId)
    {
        var userId = GetUserId();

        var inscription = await _db.Inscriptions
            .FirstOrDefaultAsync(i => i.UserId == userId && i.EvenementId == evenementId);

        if (inscription is null) return NotFound("Inscription introuvable.");

        _db.Inscriptions.Remove(inscription);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("evenement/{evenementId}")]
    public async Task<IActionResult> GetInscrits(int evenementId)
    {
        if (!await _db.Evenements.AnyAsync(e => e.Id == evenementId))
            return NotFound("Événement introuvable.");

        var inscrits = await _db.Inscriptions
            .Where(i => i.EvenementId == evenementId)
            .Include(i => i.User)
            .Select(i => new { i.User.Id, i.User.Username, i.User.Email, i.InscritLe })
            .ToListAsync();

        return Ok(inscrits);
    }
}
