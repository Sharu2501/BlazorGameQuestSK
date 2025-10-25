using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlazorGameAPI.Data;
using SharedModels.Model;

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
    /// <returns></returns>
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
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreatePlayer([FromBody] Player player)
    {
        _context.Players.Add(player);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetPlayers), new { id = player.Id }, player);
    }
}