using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlazorGameAPI.Data;
using SharedModels.Model;

[ApiController]
[Route("api/[controller]")]
public class GameHistoryController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public GameHistoryController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Permet de récupérer l'historique des parties jouées.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetGameHistories()
    {
        var histories = await _context.GameHistories.Include(gh => gh.Player).ToListAsync();
        return Ok(histories);
    }

    /// <summary>
    /// Permet de récupérer l'historique de tous les jeux jouées à partir de l'id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetGameHistory(int id)
    {
        var history = await _context.GameHistories.Include(gh => gh.Player).FirstOrDefaultAsync(gh => gh.Id == id);
        if (history == null) return NotFound();
        return Ok(history);
    }
}