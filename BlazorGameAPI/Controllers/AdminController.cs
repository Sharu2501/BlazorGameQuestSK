using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlazorGameAPI.Data;
using SharedModels.Model;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Permet d'obtenir tous les joueurs.
    /// </summary>
    /// <returns></returns>
    [HttpGet("players")]
    public async Task<IActionResult> GetAllPlayers()
    {
        var players = await _context.Players.ToListAsync();
        return Ok(players);
    }

    /// <summary>
    /// Permet de supprimer le joueur à partir de son id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("players/{id}")]
    public async Task<IActionResult> DeletePlayer(int id)
    {
        var player = await _context.Players.FindAsync(id);
        if (player == null) return NotFound();
        _context.Players.Remove(player);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}