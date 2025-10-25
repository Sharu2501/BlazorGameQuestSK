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
    /// Permet de récupérer tous les monstres.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetMonsters()
    {
        var monsters = await _context.Monsters.ToListAsync();
        return Ok(monsters);
    }

    /// <summary>
    /// Permet de créer un monstre.
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