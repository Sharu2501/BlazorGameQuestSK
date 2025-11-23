namespace BlazorGame.Client.Services
{
    public class AuthService
    {
        private int? _currentPlayerId;
        private string? _currentUsername;

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
    }
}