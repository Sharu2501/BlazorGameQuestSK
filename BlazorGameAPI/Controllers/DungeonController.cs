using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlazorGameAPI.Data;
using SharedModels.Model;

[ApiController]
[Route("api/[controller]")]
public class DungeonController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DungeonController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Permet de récupérer tous les donjons.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetDungeons()
    {
        var dungeons = await _context.Dungeons.Include(d => d.Rooms).ToListAsync();
        return Ok(dungeons);
    }

    /// <summary>
    /// Permet de récupérer le donjon à partir de son id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDungeon(int id)
    {
        var dungeon = await _context.Dungeons.Include(d => d.Rooms).FirstOrDefaultAsync(d => d.IdDungeon == id);
        if (dungeon == null) return NotFound();
        return Ok(dungeon);
    }

    /// <summary>
    /// Permet de créer un nouveau donjon.
    /// </summary>
    /// <param name="dungeon"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateDungeon([FromBody] Dungeon dungeon)
    {
        _context.Dungeons.Add(dungeon);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetDungeon), new { id = dungeon.IdDungeon }, dungeon);
    }

    /// <summary>
    /// Permet de supprimer un donjon à partir de son id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDungeon(int id)
    {
        var dungeon = await _context.Dungeons.FindAsync(id);
        if (dungeon == null) return NotFound();
        _context.Dungeons.Remove(dungeon);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}