using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlazorGameAPI.Data;
using SharedModels.Model;
using SharedModels.Model.DTOs;
using BlazorGameAPI.Services;

namespace BlazorGameAPI.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class GameSessionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly GameSessionService _svc;

        public GameSessionController(ApplicationDbContext context, GameSessionService svc)
        {
            _context = context;
            _svc = svc;
        }

        /// <summary>
        /// Permet de commencer une session.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        [HttpPost("start")]
        public async Task<IActionResult> StartSession([FromBody] GameSession session)
        {
            _context.GameSessions.Add(session);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSession), new { id = session.SessionId }, session);
        }

        /// <summary>
        ///  Permet de récupérer une session à partir de son id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSession(int id)
        {
            var session = await _context.GameSessions.FirstOrDefaultAsync(gs => gs.SessionId == id);
            if (session == null) return NotFound();
            return Ok(session);
        }

        /// <summary>
        /// Permet de terminer une session.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("end/{id}")]
        public async Task<IActionResult> EndSession(int id)
        {
            var session = await _context.GameSessions.FindAsync(id);
            if (session == null) return NotFound();
            session.IsActive = false;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Save(int id, [FromBody] SaveSessionDto dto)
        {
            var ok = await _svc.SaveSessionAsync(id, dto.StateJson, dto.IsPaused);
            if (!ok) return NotFound();
            return NoContent();
        }

    }
}