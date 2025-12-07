using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlazorGameAPI.Data;
using SharedModels.Model;

namespace BlazorGameAPI.Controllers {
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
        ///Retourne la liste des historiques de jeu avec les joueurs et donjons complétés.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetGameHistories()
        {
            var histories = await _context.GameHistories
                .Include(gh => gh.Player)
                .Include(gh => gh.CompletedDungeons)
                .ToListAsync();
            return Ok(histories);
        }
        /// <summary>
        /// Retourne un historique de jeu spécifique avec le joueur et les donjons complétés.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGameHistory(int id)
        {
            var history = await _context.GameHistories
                .Include(gh => gh.Player)
                .Include(gh => gh.CompletedDungeons)
                .FirstOrDefaultAsync(gh => gh.Id == id);
            if (history == null) return NotFound();
            return Ok(history);
        }
        /// <summary>
        /// Retourne les historiques de jeu d'un joueur spécifique.
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        [HttpGet("player/{playerId}")]
        public async Task<IActionResult> GetPlayerGameHistories(int playerId)
        {
            var histories = await _context.GameHistories
                .Include(gh => gh.Player)
                .Include(gh => gh.CompletedDungeons)
                .Where(gh => gh.Player.Id == playerId)
                .OrderByDescending(gh => gh.DatePlayed)
                .ToListAsync();
            return Ok(histories);
        }

        /// <summary>
        /// Crée un nouvel historique de jeu pour un joueur avec des donjons complétés optionnels.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateGameHistory([FromBody] CreateGameHistoryRequest request)
        {
            var player = await _context.Players.FindAsync(request.PlayerId);
            
            if (player == null) return NotFound();

            var history = new GameHistory
            {
                Player = player,
                DatePlayed = DateTime.UtcNow,
                Score = request.Score,
                CompletedDungeons = new List<Dungeon>()
            
            };

            if (request.DungeonIds != null && request.DungeonIds.Any())
            {
                var dungeons = await _context.Dungeons
                    .Where(d => request.DungeonIds.Contains(d.IdDungeon))
                    .ToListAsync();
                history.CompletedDungeons = dungeons;
            }

            _context.GameHistories.Add(history);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGameHistory), new { id = history.Id }, history);
        }
    }

    /// <summary>
    /// Requête pour créer un historique de jeu.
    /// </summary>
    public class CreateGameHistoryRequest
    {
        public int PlayerId { get; set; }
        public List<int>? DungeonIds { get; set; }
        public int Score { get; set; }
    }
} 