using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlazorGameAPI.Data;
using System.Text;
using SharedModels.Model;

namespace BlazorGameAPI.Controllers 
{
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
        [HttpGet("players")]
        public async Task<IActionResult> GetAllPlayers()
        {
            var players = await _context.Players.ToListAsync();
            return Ok(players);
        }

        /// <summary>
        /// Change le statut Actif/Inactif d'un joueur.
        /// </summary>
        [HttpPut("players/{id}/status")]
        public async Task<IActionResult> TogglePlayerStatus(int id, [FromBody] bool isActive)
        {
            var player = await _context.Players.FindAsync(id);
            if (player == null) return NotFound();

            player.IsActive = isActive;
            await _context.SaveChangesAsync();
            return Ok(player);
        }

        /// <summary>
        /// Exporte la liste des joueurs en CSV.
        /// </summary>
        [HttpGet("players/export")]
        public async Task<IActionResult> ExportPlayers()
        {
            var players = await _context.Players.ToListAsync();
            var builder = new StringBuilder();
            
            // En-tête du CSV
            builder.AppendLine("Id,Username,Email,Level,Gold,IsActive");

            foreach (var player in players)
            {
                builder.AppendLine($"{player.Id},{player.Username},{player.Email},{player.Level},{player.Gold},{player.IsActive}");
            }

            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "players_export.csv");
        }

        [HttpGet("leaderboard")]
        public async Task<IActionResult> GetLeaderboard()
        {
            var players = await _context.Players
                .Include(p => p.HighScore)
                .OrderByDescending(p => p.HighScore.Score)
                .ToListAsync();

            return Ok(players);
        }

        /// <summary>
        /// Permet de supprimer le joueur à partir de son id.
        /// </summary>
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
}