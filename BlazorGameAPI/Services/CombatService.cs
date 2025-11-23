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
        /// <summary>
        /// Constructeur du service de combat.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="playerService"></param>
        public CombatService(ApplicationDbContext context, PlayerService playerService)
        {
            _context = context;
            _playerService = playerService;
            _random = new Random();
        }
        /// <summary>
        /// Lance un dé à un nombre de faces spécifié.
        /// </summary>
        /// <param name="sides"></param>
        /// <returns></returns>
        private int RollDice(int sides = 20)
        {
            return _random.Next(1, sides + 1);
        }
        /// <summary>
        /// Calcule la probabilité de toucher en fonction des niveaux de l'attaquant et du défenseur.
        /// </summary>
        /// <param name="attackerLevel"></param>
        /// <param name="defenderLevel"></param>
        /// <returns></returns>
        private double CalculateHitChance(int attackerLevel, int defenderLevel)
        {
            int levelDifference = attackerLevel - defenderLevel;
            double baseChance = 0.75;
            double chanceModifier = levelDifference * 0.05;
            double hitChance = Math.Clamp(baseChance + chanceModifier, 0.05, 0.95);
            return hitChance;
        }
        /// <summary>
        /// Calcule les dégâts infligés en fonction de l'attaque et de la défense.
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
        public async Task<AttackResult> PlayerAttacksMonster(int playerId, int monsterId)
        {
            var player = await _context.Players.FindAsync(playerId);
            var monster = await _context.Monsters.FindAsync(monsterId);

            if (player == null || monster == null)
                return new AttackResult { Success = false, Message = "Erreur: joueur ou monstre introuvable" };

            double hitChance = CalculateHitChance(player.Level, monster.Level);
            bool hit = _random.NextDouble() < hitChance;

            if (!hit)
            {
                return new AttackResult
                {
                    Success = true,
                    Hit = false,
                    Message = "Votre attaque a échoué !"
                };
            }

            var damage = await CalculateDamage(player.Attack, monster.Defense);
            monster.Health = Math.Max(monster.Health - damage, 0);

            await _context.SaveChangesAsync();

            return new AttackResult
            {
                Success = true,
                Hit = true,
                Damage = damage,
                MonsterHealth = monster.Health,
                Message = $"Vous infligez {damage} dégâts !"
            };
        }
        /// <summary>
        /// Le monstre attaque le joueur.
        /// </summary>
        /// <param name="monsterId"></param>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public async Task<AttackResult> MonsterAttacksPlayer(int monsterId, int playerId)
        {
            var monster = await _context.Monsters.FindAsync(monsterId);
            var player = await _context.Players.FindAsync(playerId);

            if (monster == null || player == null)
                return new AttackResult { Success = false, Message = "Erreur" };

            double hitChance = CalculateHitChance(monster.Level, player.Level);
            bool hit = _random.NextDouble() < hitChance;

            if (!hit)
            {
                return new AttackResult
                {
                    Success = true,
                    Hit = false,
                    Message = "Le monstre a raté son attaque !"
                };
            }

            var damage = await CalculateDamage(monster.Attack, player.Defense);
            player.Health = Math.Max(player.Health - damage, 0);

            await _context.SaveChangesAsync();

            return new AttackResult
            {
                Success = true,
                Hit = true,
                Damage = damage,
                PlayerHealth = player.Health,
                Message = $"Le monstre vous inflige {damage} dégâts !"
            };
        }
        /// <summary>
        /// Le joueur défend pour augmenter sa défense.
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
        /// Le joueur se soigne en combat.
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
        /// Le joueur remporte le combat.
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
        /// Le joueur est vaincu.
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
    /// <summary>
    /// Résultat d'une attaque en combat.
    /// </summary>
    public class AttackResult
    {
        public bool Success { get; set; }
        public bool Hit { get; set; }
        public int Damage { get; set; }
        public int MonsterHealth { get; set; }
        public int PlayerHealth { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}