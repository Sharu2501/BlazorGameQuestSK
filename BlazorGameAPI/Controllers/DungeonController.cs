using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlazorGameAPI.Data;
using BlazorGameAPI.Services;
using SharedModels.Model;

namespace BlazorGameAPI.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class DungeonController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly DungeonService _dungeonService;

        public DungeonController(ApplicationDbContext context, DungeonService dungeonService)
        {
            _context = context;
            _dungeonService = dungeonService;
        }
        /// <summary>
        /// Retourne la liste des donjons avec leurs salles.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetDungeons()
        {
            var dungeons = await _context.Dungeons.Include(d => d.Rooms).ToListAsync();
            return Ok(dungeons);
        }
        /// <summary>
        /// Retourne un donjon spécifique avec ses salles et monstres.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDungeon(int id)
        {
            var dungeon = await _context.Dungeons.Include(d => d.Rooms).ThenInclude(r => r.Monster).FirstOrDefaultAsync(d => d.IdDungeon == id);
            if (dungeon == null) return NotFound();
            return Ok(dungeon);
        }
        /// <summary>
        /// Crée un nouveau donjon.
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
        /// Génère un donjon aléatoire avec un nombre spécifié de salles et un niveau donné.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateDungeon([FromBody] GenerateDungeonRequest request)
        {
            var dungeon = await _dungeonService.GenerateRandomDungeon(request.NumberOfRooms, request.Level);
            var fullDungeon = await _context.Dungeons
                .Include(d => d.Rooms)
                .ThenInclude(r => r.Monster)
                .FirstOrDefaultAsync(d => d.IdDungeon == dungeon.IdDungeon);
            return Ok(fullDungeon);
        }
        /// <summary>
        /// Supprime un donjon spécifique.
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
    /// <summary>
    /// Requête pour générer un donjon.
    /// </summary>
    public class GenerateDungeonRequest
    {
        public int NumberOfRooms { get; set; }
        public int Level { get; set; }
    }
}