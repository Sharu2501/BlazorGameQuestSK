using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlazorGameAPI.Data;
using SharedModels.Model;

[ApiController]
[Route("api/[controller]")]
public class HighScoreController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public HighScoreController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Permet de récupérer tous les meilleures scores.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetHighScores()
    {
        var highScores = await _context.HighScores.ToListAsync();
        return Ok(highScores);
    }

    /// <summary>
    /// Permet de récupérer les meilleures scores à partir de l'id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetHighScore(int id)
    {
        var highScore = await _context.HighScores.FirstOrDefaultAsync(hs => hs.Id == id);
        if (highScore == null) return NotFound();
        return Ok(highScore);
    }
}