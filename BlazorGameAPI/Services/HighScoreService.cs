using Microsoft.EntityFrameworkCore;
using SharedModels.Model;
using BlazorGameAPI.Data;

namespace BlazorGameAPI.Services
{
    public class HighScoreService
    {
        private readonly ApplicationDbContext _context;

        public HighScoreService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Crée un nouveau score élevé pour un joueur.
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public async Task<HighScore> CreateHighScore(int playerId, int score)
        {
            var highScore = new HighScore
            {
                Score = score,
                DateAchieved = DateTime.UtcNow
            };

            _context.HighScores.Add(highScore);
            await _context.SaveChangesAsync();

            return highScore;
        }

        /// <summary>
        /// Met à jour le score élevé d'un joueur si le nouveau score est supérieur.
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="newScore"></param>
        /// <returns></returns>
        public async Task<bool> UpdateHighScore(int playerId, int newScore)
        {
            var player = await _context.Players
                .Include(p => p.HighScore)
                .FirstOrDefaultAsync(p => p.Id == playerId);

            if (player == null)
                return false;

            if (player.HighScore == null || newScore > player.HighScore.Score)
            {
                if (player.HighScore == null)
                {
                    player.HighScore = new HighScore
                    {
                        Score = newScore,
                        DateAchieved = DateTime.UtcNow
                    };
                }
                else
                {
                    player.HighScore.Score = newScore;
                    player.HighScore.DateAchieved = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Récupère le score élevé d'un joueur.
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public async Task<HighScore?> GetPlayerHighScore(int playerId)
        {
            var player = await _context.Players
                .Include(p => p.HighScore)
                .FirstOrDefaultAsync(p => p.Id == playerId);

            return player?.HighScore;
        }

        /// <summary>
        /// Récupère les meilleurs scores.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task<List<HighScore>> GetTopScores(int count)
        {
            return await _context.HighScores
                .OrderByDescending(hs => hs.Score)
                .Take(count)
                .ToListAsync();
        }
        
        /// <summary>
        /// Récupère le rang d'un joueur.
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public async Task<int> GetPlayerRank(int playerId)
        {
            var playerScore = await GetPlayerHighScore(playerId);
            if (playerScore == null)
                return -1;

            var rank = await _context.HighScores
                .CountAsync(hs => hs.Score > playerScore.Score);

            return rank + 1;
        }
    }
}