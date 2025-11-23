using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlazorGameAPI.Data;
using SharedModels.Model;
using SharedModels.Model.DTOs;

[ApiController]
[Route("api/[controller]")]
public class PlayerController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PlayerController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Permet de récupérer tous les joueurs.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetPlayers()
    {
        var players = await _context.Players.ToListAsync();
        return Ok(players);
    }

    /// <summary>
    /// Permet de créer un joueur.
    /// </summary>
    /// <param name="player"></param>
    [HttpPost]
    public async Task<IActionResult> CreatePlayer([FromBody] Player player)
    {
        _context.Players.Add(player);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetPlayers), new { id = player.Id }, player);
    }

    /// <summary>
    /// Détail stats du joueur pour le composant Blazor.
    /// </summary>
    [HttpGet("{id}/stats")]
    public async Task<ActionResult<PlayerStatsDto>> GetPlayerStats(int id)
    {
        var player = await _context.Players.FindAsync(id);
        if (player == null)
            return NotFound();

        var dto = new PlayerStatsDto
        {
            PlayerId = player.Id,
            Username = player.Username,
            Level = player.Level,
            ExperiencePoints = player.ExperiencePoints,
            LevelCap = player.LevelCap,
            Gold = player.Gold,
            Health = player.Health,
            MaxHealth = player.MaxHealth,
            Attack = player.Attack,
            Defense = player.Defense,
        };
        return Ok(dto);
    }

    /// <summary>
    /// Supprime un joueur par son Id.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlayer(int id)
    {
        var player = await _context.Players.FindAsync(id);
        if (player == null)
            return NotFound();

        _context.Players.Remove(player);
        await _context.SaveChangesAsync();

        return NoContent();
    }

}