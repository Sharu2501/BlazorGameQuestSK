using Microsoft.EntityFrameworkCore;
using SharedModels.Model;
using SharedModels.Model.DTOs;
using SharedModels.Enum;
using BlazorGameAPI.Data;

namespace BlazorGameAPI.Services
{
    public class PlayerService
    {
        private readonly ApplicationDbContext _context;

        public PlayerService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Récupère les statistiques complètes d'un joueur
        /// </summary>
        public async Task<PlayerStatsDto?> GetPlayerStats(int playerId)
        {
            var player = await _context.Players
                .Include(p => p.HighScore)
                .Include(p => p.Inventory)
                .FirstOrDefaultAsync(p => p.PlayerId == playerId);

            if (player == null)
            {
                return null;
            }

            var gameHistory = await _context.GameHistories
                .Include(gh => gh.CompletedDungeons)
                .FirstOrDefaultAsync(gh => gh.Player.PlayerId == playerId);

            double expPercentage = player.LevelCap > 0 
                ? (player.ExperiencePoints / (double)player.LevelCap) * 100 
                : 0;

            double healthPercentage = player.MaxHealth > 0 
                ? (player.Health / (double)player.MaxHealth) * 100 
                : 0;

            int totalDungeons = gameHistory?.CompletedDungeons?.Count ?? 0;

            return new PlayerStatsDto
            {
                PlayerId = player.PlayerId,
                Username = player.Username,
                Level = player.Level,
                ExperiencePoints = player.ExperiencePoints,
                LevelCap = player.LevelCap,
                ExperiencePercentage = Math.Round(expPercentage, 2),
                Gold = player.Gold,
                Health = player.Health,
                MaxHealth = player.MaxHealth,
                HealthPercentage = Math.Round(healthPercentage, 2),
                Attack = player.Attack,
                Defense = player.Defense,
                CurrentAction = player.Action,
                InventoryCount = player.Inventory?.Count ?? 0,
                HighestScore = player.HighScore?.Score ?? 0,
                HighScoreDate = player.HighScore?.DateAchieved,
                TotalDungeonsCompleted = totalDungeons
            };
        }

        /// <summary>
        /// Ajoute de l'expérience au joueur et gère la montée de niveau
        /// </summary>
        public async Task AddExperience(int playerId, int points)
        {
            var player = await _context.Players.FindAsync(playerId);
            if (player == null) return;

            player.ExperiencePoints += points;

            while (player.ExperiencePoints >= player.LevelCap)
            {
                player.ExperiencePoints -= player.LevelCap;
                player.Level++;
                player.LevelCap += 100;
                
                player.MaxHealth += 10;
                player.Health = player.MaxHealth;
                player.Attack += 2;
                player.Defense += 1;
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Change l'action actuelle du joueur
        /// </summary>
        public async Task ChangeAction(int playerId, PlayerActionEnum action)
        {
            var player = await _context.Players.FindAsync(playerId);
            if (player == null) return;

            player.Action = action;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Réinitialise l'action du joueur à NONE
        /// </summary>
        public async Task ResetAction(int playerId)
        {
            await ChangeAction(playerId, PlayerActionEnum.NONE);
        }

        /// <summary>
        /// Ajoute de l'or au joueur
        /// </summary>
        public async Task AddGold(int playerId, int amount)
        {
            var player = await _context.Players.FindAsync(playerId);
            if (player == null) return;

            player.Gold += amount;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Retire de l'or au joueur
        /// </summary>
        public async Task<bool> RemoveGold(int playerId, int amount)
        {
            var player = await _context.Players.FindAsync(playerId);
            if (player == null || player.Gold < amount) return false;

            player.Gold -= amount;
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Soigne le joueur
        /// </summary>
        public async Task HealPlayer(int playerId, int amount)
        {
            var player = await _context.Players.FindAsync(playerId);
            if (player == null) return;

            player.Health = Math.Min(player.Health + amount, player.MaxHealth);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Inflige des dégâts au joueur
        /// </summary>
        public async Task TakeDamage(int playerId, int damage)
        {
            var player = await _context.Players.FindAsync(playerId);
            if (player == null) return;

            player.Health = Math.Max(player.Health - damage, 0);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Vérifie si le joueur est mort
        /// </summary>
        public async Task<bool> IsDead(int playerId)
        {
            var player = await _context.Players.FindAsync(playerId);
            return player == null || player.Health <= 0;
        }

        /// <summary>
        /// Ajoute un artefact à l'inventaire du joueur
        /// </summary>
        public async Task AddArtifactToInventory(int playerId, Artifact artifact)
        {
            var player = await _context.Players
                .Include(p => p.Inventory)
                .FirstOrDefaultAsync(p => p.PlayerId == playerId);

            if (player == null) return;

            player.Inventory.Add(artifact);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Retire un artefact de l'inventaire du joueur
        /// </summary>
        public async Task<bool> RemoveArtifactFromInventory(int playerId, int artifactId)
        {
            var player = await _context.Players
                .Include(p => p.Inventory)
                .FirstOrDefaultAsync(p => p.PlayerId == playerId);

            if (player == null) return false;

            var artifact = player.Inventory.FirstOrDefault(a => a.Id == artifactId);
            if (artifact == null) return false;

            player.Inventory.Remove(artifact);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Récupère un joueur par son ID
        /// </summary>
        public async Task<Player?> GetPlayerById(int playerId)
        {
            return await _context.Players
                .Include(p => p.HighScore)
                .Include(p => p.Inventory)
                .FirstOrDefaultAsync(p => p.PlayerId == playerId);
        }

        /// <summary>
        /// Crée un nouveau joueur
        /// </summary>
        public async Task<Player> CreatePlayer(string username, string email, string passwordHash)
        {
            var player = new Player
            {
                Username = username,
                Email = email,
                PasswordHash = passwordHash,
                Level = 1,
                Health = 100,
                MaxHealth = 100,
                Attack = 10,
                Defense = 5,
                Gold = 0,
                ExperiencePoints = 0,
                LevelCap = 100
            };

            _context.Players.Add(player);
            await _context.SaveChangesAsync();
            return player;
        }
    }
}