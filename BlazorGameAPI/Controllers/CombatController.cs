using Microsoft.AspNetCore.Mvc;
using BlazorGameAPI.Services;


namespace BlazorGameAPI.Controllers {

[ApiController]
[Route("api/[controller]")]
public class CombatController : ControllerBase
{
    private readonly CombatService _combatService;

    /// <summary>
    /// Constructeur du contrôleur de combat.
    /// </summary>
    /// <param name="combatService"></param>
    public CombatController(CombatService combatService)
    {
        _combatService = combatService;
    }

    /// <summary>
    /// Le joueur attaque le monstre.
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [HttpPost("attack")]
    public async Task<IActionResult> Attack([FromBody] AttackRequest req)
    {
        var ok = await _combatService.PlayerAttacksMonster(req.PlayerId, req.MonsterId);
        return Ok(ok);
    }
    /// <summary>
    /// Le monstre attaque le joueur.
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [HttpPost("monster-attack")]
    public async Task<IActionResult> MonsterAttack([FromBody] MonsterAttackRequest req)
    {
        var ok = await _combatService.MonsterAttacksPlayer(req.MonsterId, req.PlayerId);
        return Ok(ok);
    }
    /// <summary>
    /// Le joueur se défend contre une attaque.
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [HttpPost("defend")]
    public async Task<IActionResult> Defend([FromBody] DefendRequest req)
    {
        var ok = await _combatService.PlayerDefends(req.PlayerId);
        return Ok(ok);
    }
    /// <summary>
    /// Le joueur tente de fuir le combat.
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [HttpPost("flee")]
    public async Task<ActionResult<bool>> Flee([FromBody] FleeRequest req)
    {
        var flee = await _combatService.PlayerFlees(req.PlayerId);
        return Ok(flee);
    }
    /// <summary>
    /// Le joueur remporte le combat.
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [HttpPost("victory")]
    public async Task<IActionResult> Victory([FromBody] VictoryRequest req)
    {
        await _combatService.ResolveCombatVictory(req.PlayerId, req.RoomId);
        return Ok();
    }
}
    /// <summary>
    /// Requête d'attaque du joueur.
    /// </summary>
    public class AttackRequest { public int PlayerId { get; set; } public int MonsterId { get; set; } }

    /// <summary>
    /// Requête d'attaque du monstre.
    /// </summary>
    public class MonsterAttackRequest { public int MonsterId { get; set; } public int PlayerId { get; set; } }

    /// <summary>
    /// Requête de défense du joueur.
    /// </summary>
    public class DefendRequest { public int PlayerId { get; set; } }

    /// <summary>
    /// Requête de fuite du joueur.
    /// </summary>
    public class FleeRequest { public int PlayerId { get; set; } }

    /// <summary>
    /// Requête de victoire du joueur.
    /// </summary>
    public class VictoryRequest { public int PlayerId { get; set; } public int RoomId { get; set; } }
}