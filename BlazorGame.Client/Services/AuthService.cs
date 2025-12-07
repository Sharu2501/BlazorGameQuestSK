using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Net.Http.Json;
using SharedModels.Model;

namespace BlazorGame.Client.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;

        private int? _currentPlayerId;
        private string? _currentUsername;

        private string? _accessToken;
        private ClaimsPrincipal _user = new(new ClaimsIdentity());

        public AuthService(HttpClient http)
        {
            _http = http;
        }

        public int? CurrentPlayerId => _currentPlayerId;

        public string? CurrentUsername => _currentUsername;

        public string? CurrentEmail =>
            _user.FindFirst("email")?.Value
            ?? _user.FindFirst(ClaimTypes.Email)?.Value;

        public bool IsAuthenticated => _currentPlayerId.HasValue;

        public event Action? OnAuthStateChanged;

        public void Login(int playerId, string username, string accessToken)
        {
            _currentPlayerId = playerId;
            _currentUsername = username;
            _accessToken = accessToken;
            _user = CreateClaimsPrincipalFromToken(accessToken);
            OnAuthStateChanged?.Invoke();
        }

        public void LoginWithToken(string accessToken)
        {
            _accessToken = accessToken;
            _user = CreateClaimsPrincipalFromToken(accessToken);
            OnAuthStateChanged?.Invoke();
        }

        public void Logout()
        {
            _currentPlayerId = null;
            _currentUsername = null;
            _accessToken = null;
            _user = new ClaimsPrincipal(new ClaimsIdentity());
            OnAuthStateChanged?.Invoke();
        }

        public string? GetAccessToken() => _accessToken;

        private ClaimsPrincipal CreateClaimsPrincipalFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var identity = new ClaimsIdentity();

            if (handler.CanReadToken(token))
            {
                var jwt = handler.ReadJwtToken(token);
                identity = new ClaimsIdentity(jwt.Claims, "jwt");
            }

            return new ClaimsPrincipal(identity);
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