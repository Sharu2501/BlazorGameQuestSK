using System.Net.Http.Json;
using System.Text.Json;
using SharedModels.Model;

namespace BlazorGame.Client.Services;

public class GameSaveService
{
    private readonly HttpClient _http;
    public GameSaveService(HttpClient http) { _http = http; }

    public async Task<int> CreateSessionAsync(int playerId, string name, GameState state)
    {
        var dto = new { PlayerId = playerId, Name = name, StateJson = JsonSerializer.Serialize(state), IsPaused = true };
        var res = await _http.PostAsJsonAsync("http://localhost:5240/api/GameSession", dto);
        res.EnsureSuccessStatusCode();
        var created = await res.Content.ReadFromJsonAsync<GameSession>();
        return created.SessionId;
    }

    public async Task SaveSessionAsync(int sessionId, GameState state, bool isPaused)
    {
        var dto = new { StateJson = JsonSerializer.Serialize(state), IsPaused = isPaused };
        var res = await _http.PutAsJsonAsync($"http://localhost:5240/api/GameSession/{sessionId}", dto);
        res.EnsureSuccessStatusCode();
    }

    public async Task<GameState?> LoadSessionAsync(int sessionId)
    {
        var res = await _http.GetAsync($"http://localhost:5240/api/GameSession/{sessionId}");
        if (!res.IsSuccessStatusCode) return null;
        var s = await res.Content.ReadFromJsonAsync<GameSession>();
        return JsonSerializer.Deserialize<GameState>(s.StateJson);
    }

    public async Task<List<GameSession>> ListSessionsAsync(int playerId)
        => await _http.GetFromJsonAsync<List<GameSession>>($"http://localhost:5240/api/GameSession/player/{playerId}");
}