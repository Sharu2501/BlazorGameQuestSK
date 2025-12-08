using SharedModels.Model.DTOs;
using SharedModels.Model;
using System.Net.Http.Json;

namespace BlazorGame.Client.Services
{
    public class GameService
    {
        private readonly HttpClient _http;
        private readonly AuthService _authService;

        public GameService(HttpClient http, AuthService authService)
        {
            _http = http;
            _authService = authService;
        }

        public async Task<PlayerStatsDto?> GetPlayerStats()
        {
            if (_authService.CurrentPlayerId == null) return null;
            return await _http.GetFromJsonAsync<PlayerStatsDto>(
                $"http://localhost:5240/api/Player/{_authService.CurrentPlayerId.Value}/stats");
        }

        public async Task<Dungeon?> GenerateDungeon(int rooms, int level, int difficulty)
        {
            var resp = await _http.PostAsJsonAsync("http://localhost:5240/api/Dungeon/generate",
                new { numberOfRooms = rooms, level, difficulty });
            return await resp.Content.ReadFromJsonAsync<Dungeon>();
        }

        public async Task<GameSession?> StartSession(int playerId, int dungeonId)
        {
            var resp = await _http.PostAsJsonAsync("http://localhost:5240/api/GameSession/start",
                new { PlayerId = playerId, DungeonId = dungeonId });
            return await resp.Content.ReadFromJsonAsync<GameSession>();
        }

        public async Task<Monster?> GetMonster(int monsterId)
        {
            return await _http.GetFromJsonAsync<Monster>($"http://localhost:5240/api/Monster/{monsterId}");
        }

        public async Task<AttackResult?> AttackMonster(int playerId, int monsterId)
        {
            var resp = await _http.PostAsJsonAsync("http://localhost:5240/api/Combat/attack", new { playerId, monsterId });
            return await resp.Content.ReadFromJsonAsync<AttackResult>();
        }

        public async Task<AttackResult?> MonsterAttack(int monsterId, int playerId)
        {
            var resp = await _http.PostAsJsonAsync("http://localhost:5240/api/Combat/monster-attack", new { monsterId, playerId });
            return await resp.Content.ReadFromJsonAsync<AttackResult>();
        }

        public async Task Defend(int playerId)
        {
            await _http.PostAsJsonAsync("http://localhost:5240/api/Combat/defend", new { playerId });
        }

        public async Task Heal(int playerId, int amount)
        {
            await _http.PostAsJsonAsync($"http://localhost:5240/api/Player/{playerId}/heal", amount);
        }

        public async Task<bool> Flee(int playerId)
        {
            var resp = await _http.PostAsJsonAsync("http://localhost:5240/api/Combat/flee", new { playerId });
            return await resp.Content.ReadFromJsonAsync<bool>();
        }

        public async Task AddGold(int playerId, int amount)
        {
            await _http.PostAsJsonAsync($"http://localhost:5240/api/Player/{playerId}/add-gold", amount);
        }

        public async Task PostVictory(int playerId, int roomId)
        {
            await _http.PostAsJsonAsync($"http://localhost:5240/api/Combat/victory", new { playerId, roomId });
        }

        public async Task UpdateHighScore(int playerId, int score)
        {
            await _http.PostAsJsonAsync("http://localhost:5240/api/HighScore/update", new { playerId, score });
        }

        public async Task AddGameHistory(int playerId, int dungeonId, int score)
        {
            await _http.PostAsJsonAsync("http://localhost:5240/api/GameHistory", new { playerId, dungeonIds = new[] { dungeonId }, score });
        }

        public async Task EndSession(int sessionId)
        {
            await _http.PostAsJsonAsync($"http://localhost:5240/api/GameSession/end/{sessionId}", new { });
        }
    }
}