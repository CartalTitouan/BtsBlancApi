using Microsoft.AspNetCore.Authorization;
using BtsBlancApi.Data;
using BtsBlancApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BtsBlancApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EvenementController : ControllerBase
{
    private readonly AppDbContext _db;

    public EvenementController(AppDbContext db) => _db = db;

    public record EvenementDto(string Titre, string Contenu, DateTime Date, string Lieu, TypeEvenement Type);

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(EvenementDto dto)
    {
        var evenement = new Evenement
        {
            Titre = dto.Titre,
            Contenu = dto.Contenu,
            Date = dto.Date,
            Lieu = dto.Lieu,
            Type = dto.Type
        };
        _db.Evenements.Add(evenement);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = evenement.Id }, evenement);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _db.Evenements.ToListAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var evenement = await _db.Evenements.FindAsync(id);
        return evenement is null ? NotFound() : Ok(evenement);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var evenement = await _db.Evenements.FindAsync(id);
        if (evenement is null) return NotFound();
        _db.Evenements.Remove(evenement);
        await _db.SaveChangesAsync();
        return Ok("Évènement supprimé avec succès !");
    }
}
