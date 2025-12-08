using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlazorGameAPI.Data;
using System.Text;
using SharedModels.Model;
using System.Net.Http.Headers;
using System.Text.Json;

namespace BlazorGameAPI.Controllers 
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        // Injection de IConfiguration pour accéder aux réglages Keycloak
        public AdminController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // --- DTO pour la création ---
        public class CreatePlayerDto
        {
            public string Username { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        /// <summary>
        /// Crée un nouveau joueur dans Keycloak puis dans la base de données.
        /// </summary>
        [HttpPost("players")]
        public async Task<IActionResult> CreatePlayer([FromBody] CreatePlayerDto request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                return BadRequest("Nom d'utilisateur et mot de passe requis.");

            // 1. Création dans Keycloak
            string keycloakId = "";
            try 
            {
                keycloakId = await CreateUserInKeycloak(request);
                if (string.IsNullOrEmpty(keycloakId))
                    return StatusCode(500, "Erreur lors de la création Keycloak (pas d'ID retourné).");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur Keycloak : {ex.Message}");
            }

            // 2. Création dans la base de données locale
            var newPlayer = new Player
            {
                Username = request.Username,
                Email = request.Email,
                // On ne stocke pas le mot de passe hashé ici car géré par Keycloak
                // On peut mettre une valeur placeholder ou laisser vide
                PasswordHash = "MANAGED_BY_KEYCLOAK", 
                ExternalId = keycloakId,
                UserType = SharedModels.Enum.UserTypeEnum.PLAYER,
                
                // Stats par défaut
                Level = 1,
                Health = 100,
                MaxHealth = 100,
                Gold = 0,
                ExperiencePoints = 0,
                LevelCap = 100,
                Attack = 10,
                Defense = 5,
                IsActive = true
            };

            _context.Players.Add(newPlayer);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Joueur créé avec succès", id = newPlayer.Id });
        }

        private async Task<string> CreateUserInKeycloak(CreatePlayerDto user)
        {
            // Récupération configuration
            var authority = _configuration["Keycloak:Authority"]; // ex: http://localhost:8080/realms/MyRealm
            // ATTENTION : Pour l'API Admin, il faut souvent utiliser l'URL de base sans /realms/... ou construire l'url admin
            // Supposons ici que 'authority' est l'issuer complet.
            
            // Configuration client admin (à mettre dans appsettings)
            var clientId = "admin-cli"; 
            var adminUsername = "admin"; // ou via config
            var adminPassword = "admin"; // ou via config
            
            using var httpClient = new HttpClient();

            // A. Obtenir le token Admin
            // URL Token : {authority}/protocol/openid-connect/token
            var tokenUrl = $"{authority}/protocol/openid-connect/token";
            
            var tokenRequest = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("username", adminUsername),
                new KeyValuePair<string, string>("password", adminPassword),
                new KeyValuePair<string, string>("grant_type", "password")
            });

            var tokenResponse = await httpClient.PostAsync(tokenUrl, tokenRequest);
            if (!tokenResponse.IsSuccessStatusCode) 
                throw new Exception($"Impossible d'obtenir le token admin ({tokenResponse.StatusCode})");
            
            var tokenContent = await tokenResponse.Content.ReadFromJsonAsync<JsonElement>();
            var accessToken = tokenContent.GetProperty("access_token").GetString();

            // B. Créer l'utilisateur
            // URL Admin Users : souvent {BaseUrl}/admin/realms/{RealmName}/users
            // Astuce : on remplace /realms/ par /admin/realms/ dans l'URL d'autorité
            var adminUsersUrl = authority.Replace("/realms/", "/admin/realms/") + "/users";

            var newUserPayload = new
            {
                username = user.Username,
                email = user.Email,
                firstName = user.FirstName,
                lastName = user.LastName,
                enabled = true,
                emailVerified = true,
                credentials = new[] 
                {
                    new { type = "password", value = user.Password, temporary = false }
                }
            };

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var createUserResponse = await httpClient.PostAsJsonAsync(adminUsersUrl, newUserPayload);

            if (!createUserResponse.IsSuccessStatusCode)
            {
                var error = await createUserResponse.Content.ReadAsStringAsync();
                throw new Exception($"Erreur création user Keycloak: {createUserResponse.StatusCode} - {error}");
            }

            // C. Récupérer l'ID du user créé (via Location header ou recherche)
            if (createUserResponse.Headers.Location != null)
            {
                // L'ID est souvent à la fin de l'URL Location
                return createUserResponse.Headers.Location.Segments.Last();
            }
            
            return "UNKNOWN_ID"; // Fallback si l'ID n'est pas récupérable immédiatement
        }

        // --- Méthodes existantes ---

        /// <summary>
        /// Permet d'obtenir tous les joueurs.
        /// </summary>
        [HttpGet("players")]
        public async Task<IActionResult> GetAllPlayers()
        {
            var players = await _context.Players.ToListAsync();
            return Ok(players);
        }

        /// <summary>
        /// Change le statut Actif/Inactif d'un joueur.
        /// </summary>
        [HttpPut("players/{id}/status")]
        public async Task<IActionResult> TogglePlayerStatus(int id, [FromBody] bool isActive)
        {
            var player = await _context.Players.FindAsync(id);
            if (player == null) return NotFound();

            player.IsActive = isActive;
            await _context.SaveChangesAsync();
            return Ok(player);
        }

        /// <summary>
        /// Exporte la liste des joueurs en CSV.
        /// </summary>
        [HttpGet("players/export")]
        public async Task<IActionResult> ExportPlayers()
        {
            var players = await _context.Players.ToListAsync();
            var builder = new StringBuilder();
            
            // En-tête du CSV
            builder.AppendLine("Id,Username,Email,Level,Gold,IsActive");

            foreach (var player in players)
            {
                builder.AppendLine($"{player.Id},{player.Username},{player.Email},{player.Level},{player.Gold},{player.IsActive}");
            }

            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "players_export.csv");
        }

        [HttpGet("leaderboard")]
        public async Task<IActionResult> GetLeaderboard()
        {
            var players = await _context.Players
                .Include(p => p.HighScore)
                .OrderByDescending(p => p.HighScore.Score)
                .ToListAsync();

            return Ok(players);
        }

        /// <summary>
        /// Permet de supprimer le joueur à partir de son id.
        /// </summary>
        [HttpDelete("players/{id}")]
        public async Task<IActionResult> DeletePlayer(int id)
        {
            var player = await _context.Players.FindAsync(id);
            if (player == null) return NotFound();
            _context.Players.Remove(player);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}