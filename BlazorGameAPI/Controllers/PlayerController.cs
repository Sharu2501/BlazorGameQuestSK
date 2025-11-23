using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlazorGameAPI.Data;
using SharedModels.Model;
using SharedModels.Model.DTOs;
using BlazorGameAPI.Services;

[ApiController]
[Route("api/[controller]")]
public class PlayerController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly PlayerService _playerService;
/// <summary>
/// Constructeur du contrôleur des joueurs.
/// </summary>
/// <param name="context"></param>
/// <param name="playerService"></param>
    public PlayerController(ApplicationDbContext context, PlayerService playerService)
    {
        _context = context;
        _playerService = playerService;
    }
/// <summary>
/// Retourne la liste des joueurs.
/// </summary>
/// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetPlayers()
    {
        var players = await _context.Players.ToListAsync();
        return Ok(players);
    }
/// <summary>
/// Crée un nouveau joueur.
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
/// <summary>
/// Retourne les statistiques d'un joueur spécifique.
/// </summary>
/// <param name="id"></param>
/// <returns></returns>
    [HttpGet("{id}/stats")]
    public async Task<ActionResult<PlayerStatsDto>> GetPlayerStats(int id)
    {
        var stats = await _playerService.GetPlayerStats(id);
        if (stats == null)
            return NotFound();
        return Ok(stats);
    }
/// <summary>
/// Soigne un joueur spécifique.
/// </summary>
/// <param name="id"></param>
/// <param name="amount"></param>
/// <returns></returns>
    [HttpPost("{id}/heal")]
    public async Task<IActionResult> HealPlayer(int id, [FromBody] int amount)
    {
        await _playerService.HealPlayer(id, amount);
        return Ok();
    }
/// <summary>
/// Ajoute de l'or à un joueur spécifique.
/// </summary>
/// <param name="id"></param>
/// <param name="amount"></param>
/// <returns></returns>
    [HttpPost("{id}/add-gold")]
    public async Task<IActionResult> AddGold(int id, [FromBody] int amount)
    {
        await _playerService.AddGold(id, amount);
        return Ok();
    }
/// <summary>
/// Supprime un joueur spécifique.
/// </summary>
/// <param name="id"></param>
/// <returns></returns>
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