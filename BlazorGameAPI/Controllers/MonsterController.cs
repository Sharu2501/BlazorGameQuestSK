using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlazorGameAPI.Data;
using SharedModels.Model;

[ApiController]
[Route("api/[controller]")]
public class MonsterController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public MonsterController(ApplicationDbContext context)
    {
        _context = context;
    }
/// <summary>
/// Retourne la liste des monstres.
/// </summary>
/// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetMonsters()
    {
        var monsters = await _context.Monsters.ToListAsync();
        return Ok(monsters);
    }
/// <summary>
/// Retourne un monstre spécifique.
/// </summary>
/// <param name="id"></param>
/// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetMonster(int id)
    {
        var monster = await _context.Monsters.FindAsync(id);
        if (monster == null) return NotFound();
        return Ok(monster);
    }
/// <summary>
/// Crée un nouveau monstre.
/// </summary>
/// <param name="monster"></param>
/// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateMonster([FromBody] Monster monster)
    {
        _context.Monsters.Add(monster);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetMonsters), new { id = monster.IdMonster }, monster);
    }
}