using System.Net.Http.Json;
using SharedModels.Model;

namespace BlazorGame.Client.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;

        private int? _currentPlayerId;
        private string? _currentUsername;

        public AuthService(HttpClient http)
        {
            _http = http;
        }

        public int? CurrentPlayerId => _currentPlayerId;
        public string? CurrentUsername => _currentUsername;
        public bool IsAuthenticated => _currentPlayerId.HasValue;

        public event Action? OnAuthStateChanged;
        
        /// <summary>
        /// Connecte un joueur avec l'ID et le nom d'utilisateur spécifiés.
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="username"></param>
        public void Login(int playerId, string username)
        {
            _currentPlayerId = playerId;
            _currentUsername = username;
            OnAuthStateChanged?.Invoke();
        }

        /// <summary>
        /// Déconnecte le joueur actuel.
        /// </summary>
        public void Logout()
        {
            _currentPlayerId = null;
            _currentUsername = null;
            OnAuthStateChanged?.Invoke();
        }

        public async Task<User?> GetCurrentUserAsync()
        {
            if (!_currentPlayerId.HasValue)
                return null;

            try
            {
                var player = await _http.GetFromJsonAsync<Player>(
                    $"http://localhost:5240/api/Player/{_currentPlayerId.Value}");

                return player;
            }
            catch
            {
                return null;
            }
        }
    }
}