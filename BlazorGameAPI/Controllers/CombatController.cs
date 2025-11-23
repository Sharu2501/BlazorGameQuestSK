using Microsoft.AspNetCore.Mvc;
using BlazorGameAPI.Services;

[ApiController]
[Route("api/[controller]")]
public class CombatController : ControllerBase
{
    private readonly CombatService _combatService;

    public CombatController(CombatService combatService)
    {
        _combatService = combatService;
    }

    [HttpPost("attack")]
    public async Task<IActionResult> Attack([FromBody] AttackRequest req)
    {
        var ok = await _combatService.PlayerAttacksMonster(req.PlayerId, req.MonsterId);
        return Ok(ok);
    }

    [HttpPost("flee")]
    public async Task<ActionResult<bool>> Flee([FromBody] FleeRequest req)
    {
        var flee = await _combatService.PlayerFlees(req.PlayerId);
        return Ok(flee);
    }
}

// Les DTOs simples pour la requête :
public class AttackRequest { public int PlayerId { get; set; } public int MonsterId { get; set; } }
public class FleeRequest { public int PlayerId { get; set; } }