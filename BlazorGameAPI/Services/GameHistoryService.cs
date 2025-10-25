using Microsoft.EntityFrameworkCore;
using SharedModels.Model;
using BlazorGameAPI.Data;

namespace BlazorGameAPI.Services
{
    public class GameHistoryService
    {
        private readonly ApplicationDbContext _context;

        public GameHistoryService(ApplicationDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Crée un nouvel historique de jeu pour un joueur.
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<GameHistory> CreateGameHistory(int playerId)
        {
            var player = await _context.Players.FindAsync(playerId);
            if (player == null)
                throw new Exception("Player not found");

            var history = new GameHistory
            {
                Player = player,
                CompletedDungeons = new List<Dungeon>(),
                DatePlayed = DateTime.UtcNow
            };

            _context.GameHistories.Add(history);
            await _context.SaveChangesAsync();

            return history;
        }
        /// <summary>
        /// Récupère l'historique de jeu d'un joueur par son ID.
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public async Task<GameHistory?> GetGameHistoryByPlayerId(int playerId)
        {
            return await _context.GameHistories
                .Include(gh => gh.Player)
                .Include(gh => gh.CompletedDungeons)
                .FirstOrDefaultAsync(gh => gh.Player.Id == playerId);
        }
        /// <summary>
        /// Ajoute un donjon complété à l'historique de jeu d'un joueur.
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="dungeonId"></param>
        /// <returns></returns>
        public async Task<bool> AddCompletedDungeon(int playerId, int dungeonId)
        {
            var history = await GetGameHistoryByPlayerId(playerId);

            if (history == null)
            {
                history = await CreateGameHistory(playerId);
            }

            var dungeon = await _context.Dungeons.FindAsync(dungeonId);
            if (dungeon == null)
                return false;

            if (!history.CompletedDungeons.Any(d => d.IdDungeon == dungeonId))
            {
                history.CompletedDungeons.Add(dungeon);
                history.DatePlayed = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            return true;
        }
        /// <summary>
        /// Récupère le nombre total de donjons complétés par un joueur.
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public async Task<int> GetTotalDungeonsCompleted(int playerId)
        {
            var history = await GetGameHistoryByPlayerId(playerId);
            return history?.CompletedDungeons?.Count ?? 0;
        }
        /// <summary>
        /// Récupère la liste des donjons complétés par un joueur.
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public async Task<List<Dungeon>> GetCompletedDungeons(int playerId)
        {
            var history = await GetGameHistoryByPlayerId(playerId);
            return history?.CompletedDungeons ?? new List<Dungeon>();
        }
        /// <summary>
        /// Récupère la date de la dernière partie jouée par un joueur.
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public async Task<DateTime?> GetLastPlayedDate(int playerId)
        {
            var history = await GetGameHistoryByPlayerId(playerId);
            return history?.DatePlayed;
        }
    }
}