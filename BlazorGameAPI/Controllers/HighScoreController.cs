using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlazorGameAPI.Data;
using BlazorGameAPI.Services;

namespace BlazorGameAPI.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class HighScoreController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly HighScoreService _highScoreService;

        /// <summary>
        /// Constructeur du contrôleur des scores élevés.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="highScoreService"></param>
        public HighScoreController(ApplicationDbContext context, HighScoreService highScoreService)
        {
            _context = context;
            _highScoreService = highScoreService;
        }

        /// <summary>
        /// Retourne la liste des scores élevés.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetHighScores()
        {
            var highScores = await _context.HighScores.ToListAsync();
            return Ok(highScores);
        }

        /// <summary>
        /// Retourne un score élevé spécifique.
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
        
        /// <summary>
        /// Met à jour le score élevé d'un joueur.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("update")]
        public async Task<IActionResult> UpdateHighScore([FromBody] UpdateHighScoreRequest request)
        {
            var updated = await _highScoreService.UpdateHighScore(request.PlayerId, request.Score);
            return Ok(updated);
        }
    }

    /// <summary>
    /// Requête pour mettre à jour un score élevé.
    /// </summary>
    public class UpdateHighScoreRequest
    {
        public int PlayerId { get; set; }
        public int Score { get; set; }
    }
}