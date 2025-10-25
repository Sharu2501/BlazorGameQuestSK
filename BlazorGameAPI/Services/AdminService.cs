using Microsoft.EntityFrameworkCore;
using SharedModels.Model;
using BlazorGameAPI.Data;

namespace BlazorGameAPI.Services
{
    public class AdminService
    {
        private readonly ApplicationDbContext _context;
        private readonly GameSessionService _gameSessionService;

        public AdminService(ApplicationDbContext context, GameSessionService gameSessionService)
        {
            _context = context;
            _gameSessionService = gameSessionService;
        }

        /// <summary>
        /// Récupère tous les joueurs.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Player>> GetAllPlayers()
        {
            return await _context.Players
                .Include(p => p.HighScore)
                .ToListAsync();
        }
        /// <summary>
        /// Récupère tous les administrateurs.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Admin>> GetAllAdmins()
        {
            return await _context.Admins.ToListAsync();
        }
        /// <summary>
        /// Supprime un joueur par son ID.
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public async Task<bool> DeletePlayer(int playerId)
        {
            var player = await _context.Players.FindAsync(playerId);
            if (player == null)
                return false;

            _context.Players.Remove(player);
            await _context.SaveChangesAsync();

            return true;
        }
        /// <summary>
        /// Récupère des statistiques globales sur le jeu.
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, int>> GetGlobalStatistics()
        {
            var stats = new Dictionary<string, int>
            {
                { "TotalPlayers", await _context.Players.CountAsync() },
                { "TotalAdmins", await _context.Admins.CountAsync() },
                { "TotalMonsters", await _context.Monsters.CountAsync() },
                { "TotalDungeons", await _context.Dungeons.CountAsync() },
                { "TotalRooms", await _context.Rooms.CountAsync() },
                { "TotalArtifacts", await _context.Artifacts.CountAsync() },
                { "ActiveSessions", await _context.GameSessions.CountAsync(gs => gs.IsActive) },
                { "CompletedDungeons", await _context.GameHistories.SumAsync(gh => gh.CompletedDungeons.Count) }
            };

            return stats;
        }
        /// <summary>
        /// Récupère des statistiques détaillées pour un joueur donné.
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public async Task<object> GetPlayerDetailedStats(int playerId)
        {
            var player = await _context.Players
                .Include(p => p.HighScore)
                .Include(p => p.Inventory)
                .FirstOrDefaultAsync(p => p.PlayerId == playerId);

            if (player == null)
                return null;

            var history = await _context.GameHistories
                .Include(gh => gh.CompletedDungeons)
                .FirstOrDefaultAsync(gh => gh.Player.PlayerId == playerId);

            return new
            {
                Player = player,
                TotalDungeonsCompleted = history?.CompletedDungeons?.Count ?? 0,
                InventorySize = player.Inventory?.Count ?? 0,
                HighScore = player.HighScore?.Score ?? 0,
                LastPlayed = history?.DatePlayed
            };
        }
        /// <summary>
        /// Récupère toutes les sessions de jeu actives.
        /// </summary>
        /// <returns></returns>
        public async Task<List<GameSession>> GetAllActiveSessions()
        {
            return await _gameSessionService.GetAllActiveSessions();
        }
        /// <summary>
        /// Force la fin d'une session de jeu donnée.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public async Task<bool> ForceEndSession(int sessionId)
        {
            return await _gameSessionService.EndSession(sessionId);
        }
    }
}