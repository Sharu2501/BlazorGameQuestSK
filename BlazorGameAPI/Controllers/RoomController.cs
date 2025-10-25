using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlazorGameAPI.Data;
using SharedModels.Model;

[ApiController]
[Route("api/[controller]")]
public class RoomController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RoomController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Permet de récupérer toutes les salles.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetRooms()
    {
        var rooms = await _context.Rooms.Include(r => r.Monster).ToListAsync();
        return Ok(rooms);
    }

    /// <summary>
    /// Permet de récupérer une salle à partir de son id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRoom(int id)
    {
        var room = await _context.Rooms.Include(r => r.Monster).FirstOrDefaultAsync(r => r.Id == id);
        if (room == null) return NotFound();
        return Ok(room);
    }

    /// <summary>
    /// Permet de créer une salle.
    /// </summary>
    /// <param name="room"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateRoom([FromBody] Room room)
    {
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetRoom), new { id = room.Id }, room);
    }

    /// <summary>
    /// Permet de supprimer une salle.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRoom(int id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null) return NotFound();
        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}