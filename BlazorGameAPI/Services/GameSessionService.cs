using Microsoft.EntityFrameworkCore;
using SharedModels.Model;
using BlazorGameAPI.Data;

namespace BlazorGameAPI.Services
{
    public class GameSessionService
    {
        private readonly ApplicationDbContext _context;

        public GameSessionService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Démarre une nouvelle session de jeu pour un joueur dans un donjon spécifié.
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="dungeonId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<GameSession> StartSession(int playerId, int dungeonId)
        {
            var existingSession = await GetActiveSession(playerId);
            if (existingSession != null)
            {
                await EndSession(existingSession.SessionId);
            }

            var dungeon = await _context.Dungeons
                .Include(d => d.Rooms)
                .FirstOrDefaultAsync(d => d.IdDungeon == dungeonId);

            if (dungeon == null || dungeon.Rooms.Count == 0)
                throw new Exception("Dungeon not found or has no rooms");

            var firstRoom = dungeon.Rooms.First();

            var session = new GameSession
            {
                PlayerId = playerId,
                CurrentDungeonId = dungeonId,
                CurrentRoomId = firstRoom.Id,
                IsActive = true,
                StartTime = DateTime.UtcNow
            };

            _context.GameSessions.Add(session);
            await _context.SaveChangesAsync();

            return session;
        }
        /// <summary>
        /// Récupère la session de jeu active pour un joueur donné.
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public async Task<GameSession?> GetActiveSession(int playerId)
        {
            return await _context.GameSessions
                .FirstOrDefaultAsync(gs => gs.PlayerId == playerId && gs.IsActive);
        }
        /// <summary>
        /// Récupère une session de jeu par son ID.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public async Task<GameSession?> GetSessionById(int sessionId)
        {
            return await _context.GameSessions
                .FirstOrDefaultAsync(gs => gs.SessionId == sessionId);
        }
        /// <summary>
        /// Déplace le joueur vers une nouvelle salle dans la session de jeu.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="roomId"></param>
        /// <returns></returns>
        public async Task<bool> MoveToRoom(int sessionId, int roomId)
        {
            var session = await GetSessionById(sessionId);
            if (session == null || !session.IsActive)
                return false;

            session.CurrentRoomId = roomId;
            await _context.SaveChangesAsync();

            return true;
        }
        /// <summary>
        /// Met fin à une session de jeu.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public async Task<bool> EndSession(int sessionId)
        {
            var session = await GetSessionById(sessionId);
            if (session == null)
                return false;

            session.IsActive = false;
            await _context.SaveChangesAsync();

            return true;
        }
        /// <summary>
        /// Vérifie si une session de jeu est active.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public async Task<bool> IsSessionActive(int sessionId)
        {
            var session = await GetSessionById(sessionId);
            return session?.IsActive ?? false;
        }
        /// <summary>
        /// Récupère toutes les sessions de jeu actives.
        /// </summary>
        /// <returns></returns>
        public async Task<List<GameSession>> GetAllActiveSessions()
        {
            return await _context.GameSessions
                .Where(gs => gs.IsActive)
                .ToListAsync();
        }
        /// <summary>
        /// Met à jour une session de jeu existante.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public async Task<bool> UpdateSession(GameSession session)
        {
            var existingSession = await GetSessionById(session.SessionId);
            if (existingSession == null)
                return false;

            _context.Entry(existingSession).CurrentValues.SetValues(session);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}