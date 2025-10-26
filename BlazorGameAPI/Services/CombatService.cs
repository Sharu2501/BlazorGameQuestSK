using Microsoft.EntityFrameworkCore;
using SharedModels.Model;
using BlazorGameAPI.Data;
using SharedModels.Enum;

namespace BlazorGameAPI.Services
{
    public class CombatService
    {
        private readonly ApplicationDbContext _context;
        private readonly PlayerService _playerService;
        private readonly Random _random;

        public CombatService(ApplicationDbContext context, PlayerService playerService)
        {
            _context = context;
            _playerService = playerService;
            _random = new Random();
        }
        /// <summary>
        /// Lance un dé à un nombre spécifié de faces.
        /// </summary>
        /// <param name="sides"></param>
        /// <returns></returns>
        private int RollDice(int sides = 20)
        {
            return _random.Next(1, sides + 1);
        }
        /// <summary>
        /// Calcule les dégâts infligés en fonction de l'attaque et de la défense avec des modificateurs aléatoires.
        /// </summary>
        /// <param name="attack"></param>
        /// <param name="defense"></param>
        /// <returns></returns>
        public Task<int> CalculateDamage(int attack, int defense)
        {
            var roll = RollDice(20);
            var baseDamage = attack - (defense / 2);

            int result;
            if (roll == 20) result = baseDamage * 2;
            else if (roll == 1) result = 0;
            else if (roll >= 15) result = (int)(baseDamage * 1.5);
            else if (roll <= 5) result = (int)(baseDamage * 0.5);
            else result = Math.Max(baseDamage, 1);

            return Task.FromResult(result);
        }
        /// <summary>
        /// Le joueur attaque un monstre.
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="monsterId"></param>
        /// <returns></returns>
        public async Task<bool> PlayerAttacksMonster(int playerId, int monsterId)
        {
            var player = await _context.Players.FindAsync(playerId);
            var monster = await _context.Monsters.FindAsync(monsterId);

            if (player == null || monster == null)
                return false;

            var attackRoll = RollDice(20);

            if (attackRoll == 1)
            {
                return true;
            }

            var damage = await CalculateDamage(player.Attack, monster.Defense);
            monster.Health = Math.Max(monster.Health - damage, 0);

            await _context.SaveChangesAsync();
            return true;
        }
        /// <summary>
        /// Le monstre attaque le joueur.
        /// </summary>
        /// <param name="monsterId"></param>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public async Task<bool> MonsterAttacksPlayer(int monsterId, int playerId)
        {
            var monster = await _context.Monsters.FindAsync(monsterId);
            var player = await _context.Players.FindAsync(playerId);

            if (monster == null || player == null)
                return false;

            var attackRoll = RollDice(20);

            if (attackRoll == 1)
            {
                return true;
            }

            var damage = await CalculateDamage(monster.Attack, player.Defense);
            player.Health = Math.Max(player.Health - damage, 0);

            await _context.SaveChangesAsync();
            return true;
        }
        /// <summary>
        /// Le joueur se défend pour augmenter temporairement sa défense.
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public async Task<bool> PlayerDefends(int playerId)
        {
            var player = await _context.Players.FindAsync(playerId);
            if (player == null)
                return false;

            var defenseRoll = RollDice(20);
            var bonusDefense = defenseRoll >= 10 ? defenseRoll / 2 : 0;

            player.Defense += bonusDefense;
            await _context.SaveChangesAsync();

            return true;
        }
        /// <summary>
        /// Le joueur se soigne pendant le combat.
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="healAmount"></param>
        /// <returns></returns>
        public async Task<bool> PlayerHealsInCombat(int playerId, int healAmount)
        {
            var player = await _context.Players.FindAsync(playerId);
            if (player == null)
                return false;

            var healRoll = RollDice(20);
            var actualHeal = healAmount;

            if (healRoll >= 18)
            {
                actualHeal = (int)(healAmount * 1.5);
            }
            else if (healRoll <= 3)
            {
                actualHeal = (int)(healAmount * 0.5);
            }

            player.Health = Math.Min(player.Health + actualHeal, player.MaxHealth);
            await _context.SaveChangesAsync();

            return true;
        }
        /// <summary>
        /// Le joueur tente de fuir le combat.
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public async Task<bool> PlayerFlees(int playerId)
        {
            var player = await _context.Players.FindAsync(playerId);
            if (player == null)
                return false;

            var fleeRoll = RollDice(20);
            var levelBonus = player.Level / 5;

            return fleeRoll + levelBonus >= 12;
        }
        /// <summary>
        /// Vérifie si un monstre est vaincu.
        /// </summary>
        /// <param name="monsterId"></param>
        /// <returns></returns>
        public async Task<bool> IsMonsterDefeated(int monsterId)
        {
            var monster = await _context.Monsters.FindAsync(monsterId);
            return monster == null || monster.Health <= 0;
        }
        /// <summary>
        /// Vérifie si un joueur est vaincu.
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public async Task<bool> IsPlayerDefeated(int playerId)
        {
            return await _playerService.IsDead(playerId);
        }
        /// <summary>
        /// Résout les conséquences de la victoire du joueur en combat.
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="roomId"></param>
        /// <returns></returns>
        public async Task ResolveCombatVictory(int playerId, int roomId)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null)
                return;

            var difficultyMultiplier = room.DifficultyLevel switch
            {
                DifficultyLevelEnum.EASY => 1.0,
                DifficultyLevelEnum.MEDIUM => 1.25,
                DifficultyLevelEnum.HARD => 1.5,
                DifficultyLevelEnum.EXTREME => 2.0,
                _ => 1.0
            };

            var bonusRoll = RollDice(20);
            var randomBonus = bonusRoll >= 15 ? 0.2 : 0;

            var totalExpMultiplier = difficultyMultiplier + randomBonus;
            var totalGoldMultiplier = difficultyMultiplier + randomBonus;

            var finalExp = (int)(room.ExperienceGained * totalExpMultiplier);
            var finalGold = (int)(room.GoldGained * totalGoldMultiplier);

            await _playerService.AddExperience(playerId, finalExp);
            await _playerService.AddGold(playerId, finalGold);

            room.IsExplored = true;
            await _context.SaveChangesAsync();
        }
        /// <summary>
        /// Résout les conséquences de la défaite du joueur en combat.
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="roomId"></param>
        /// <returns></returns>
        public async Task ResolveCombatDefeat(int playerId, int roomId)
        {
            var player = await _context.Players.FindAsync(playerId);
            var room = await _context.Rooms.FindAsync(roomId);

            if (player == null || room == null)
                return;

            var goldLossPercentage = room.DifficultyLevel switch
            {
                DifficultyLevelEnum.EASY => 0.05,
                DifficultyLevelEnum.MEDIUM => 0.10,
                DifficultyLevelEnum.HARD => 0.15,
                DifficultyLevelEnum.EXTREME => 0.25,
                _ => 0.10
            };

            var healthRestorePercentage = room.DifficultyLevel switch
            {
                DifficultyLevelEnum.EASY => 0.5,
                DifficultyLevelEnum.MEDIUM => 0.3,
                DifficultyLevelEnum.HARD => 0.2,
                DifficultyLevelEnum.EXTREME => 0.1,
                _ => 0.3
            };

            var goldLost = (int)(player.Gold * goldLossPercentage);
            await _playerService.RemoveGold(playerId, goldLost);

            player.Health = (int)(player.MaxHealth * healthRestorePercentage);
            await _context.SaveChangesAsync();
        }
    }
}