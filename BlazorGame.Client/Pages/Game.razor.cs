using Microsoft.AspNetCore.Components;
using BlazorGame.Client.Services;
using SharedModels.Enum;
using SharedModels.Model;
using SharedModels.Model.DTOs;
namespace BlazorGame.Client.Pages;

public partial class Game : ComponentBase
{
    [SupplyParameterFromQuery] public int Difficulty { get; set; } = 0;
    [SupplyParameterFromQuery(Name = "sessionId")]
    public int? QuerySessionId { get; set; }
    private GameState? gameState;
    private PlayerStatsDto? PlayerStats;
    private string message = "";
    private bool isLoading = true;
    private bool isPaused = false;
    private bool showPausePopup = false;

    [Inject] protected AuthService AuthService { get; set; }
    [Inject] protected GameService GameService { get; set; }
    [Inject] protected GameSaveService GameSaveService { get; set; }
    [Inject] protected NavigationManager NavigationManager { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (!AuthService.IsAuthenticated || !AuthService.CurrentPlayerId.HasValue)
        {
            NavigationManager.NavigateTo("/");
            return;
        }

        var playerId = AuthService.CurrentPlayerId.Value;

        if (QuerySessionId.HasValue)
        {
            await LoadExistingSession(QuerySessionId.Value);
            return;
        }

        var activeSession = await GameSaveService.GetActiveSessionAsync(playerId);
        if (activeSession != null)
        {
            await LoadExistingSession(activeSession.SessionId);
            return;
        }

        await InitializeGame();
    }

    private async Task LoadExistingSession(int sessionId)
    {
        try
        {
            isLoading = true;
            var state = await GameSaveService.LoadSessionAsync(sessionId);
            if (state == null)
            {
                message = "Impossible de charger la session, une nouvelle partie va commencer.";
                await InitializeGame();
                return;
            }

            gameState = state;
            isPaused = false;
            message = "Partie reprise.";
            PlayerStats = await GameService.GetPlayerStats();

            isLoading = false;
            StateHasChanged();
        }
        catch (Exception ex)
        {
            message = $"Erreur lors du chargement de la session: {ex.Message}";
            isLoading = false;
        }
    }

    private async Task InitializeGame()
    {
        try
        {
            PlayerStats = await GameService.GetPlayerStats();
            var difficultyLevel = (DifficultyLevelEnum)Difficulty;

            var dungeon = await GameService.GenerateDungeon(5, PlayerStats.Level, Difficulty);
            
            var session = await GameService.StartSession(
                AuthService.CurrentPlayerId.Value,
                dungeon.IdDungeon);
            
            gameState = new GameState
            {
                CurrentDungeon = dungeon,
                CurrentRoom = dungeon.Rooms?.FirstOrDefault(),
                CurrentRoomIndex = 0,
                TotalRooms = dungeon.Rooms?.Count ?? 0,
                IsMonsterDefeated = false,
                IsRoomCompleted = false,
                Score = 0,
                SessionId = session.SessionId,
                HealUsedInRoom = 0,
                DifficultyLevel = difficultyLevel
            };

            session = await GameService.StartSession(
                AuthService.CurrentPlayerId.Value,
                dungeon.IdDungeon);
            gameState.SessionId = session.SessionId;

            message = "Vous entrez dans le donjon...";
            isLoading = false;
        }
        catch (HttpRequestException httpEx)
        {
            message = $"Une erreur est survenue: {httpEx.Message}";
            isLoading = false;
        }
        catch (Exception ex)
        {
            message = $"Une erreur est survenue: {ex.Message}";
            isLoading = false;
        }
    }

    private async Task PauseGame()
    {
        if (gameState == null || !AuthService.CurrentPlayerId.HasValue)
            return;

        try
        {
            if (gameState.SessionId == 0)
            {
                var sessionId = await GameSaveService.CreateSessionAsync(
                    AuthService.CurrentPlayerId.Value,
                    gameState.CurrentDungeon?.Name ?? "Partie",
                    gameState
                );

                gameState.SessionId = sessionId;
            }
            else
            {
                await GameSaveService.SaveSessionAsync(gameState.SessionId, gameState, true);
            }

            isPaused = true;
            showPausePopup = true;
        }
        catch (Exception ex)
        {
            message = $"Erreur lors de la mise en pause: {ex.Message}";
        }
    }

    private void ClosePausePopup()
    {
        showPausePopup = false;
    }

    private async Task ResumeGame()
    {
        if (gameState == null || gameState.SessionId == 0)
            return;

        try
        {
            var state = await GameSaveService.LoadSessionAsync(gameState.SessionId);
            if (state != null)
            {
                gameState = state;
                isPaused = false;
                showPausePopup = false;
            }
        }
        catch (Exception ex)
        {
            message = $"Erreur lors de la reprise: {ex.Message}";
        }
    }

    private List<string> GetAvailableActions()
    {
        var actions = new List<string>();

        if (gameState == null || gameState.CurrentRoom == null)
            return actions;

        if (gameState.CurrentRoom.Monster != null && !gameState.IsMonsterDefeated)
        {
            actions.Add("Attaquer");
            actions.Add("Défendre");
            actions.Add("Soigner");
            actions.Add("Fuir");
        }
        else if (!gameState.IsRoomCompleted)
        {
            actions.Add("Fouiller");
        }
        else
        {
            if (gameState.CurrentRoomIndex < gameState.TotalRooms - 1)
            {
                actions.Add("Salle suivante");
            }
            else
            {
                actions.Add("Terminer le donjon");
            }
        }

        return actions;
    }

    private bool IsActionDisabled(string action)
    {
        if (action == "Soigner" && gameState.HealUsedInRoom >= gameState.MaxHealPerRoom)
        {
            return true;
        }
        return false;
    }

    private async Task ExecuteAction(string action)
    {
        if (isPaused)
        {
            message = "La partie est en pause. Cliquez sur Reprendre pour continuer.";
            return;
        }

        if (gameState == null || gameState.CurrentRoom == null || !AuthService.CurrentPlayerId.HasValue)
            return;

        var playerId = AuthService.CurrentPlayerId.Value;

        try
        {
            switch (action)
            {
                case "Attaquer":
                    await AttackMonster(playerId);
                    break;
                case "Défendre":
                    await Defend(playerId);
                    break;
                case "Soigner":
                    await Heal(playerId);
                    break;
                case "Fuir":
                    await Flee(playerId);
                    break;
                case "Fouiller":
                    await SearchRoom(playerId);
                    break;
                case "Salle suivante":
                    await NextRoom();
                    break;
                case "Terminer le donjon":
                    await CompleteDungeon();
                    break;
            }

            PlayerStats = await GameService.GetPlayerStats();

            if (PlayerStats?.Health <= 0)
            {
                await GameOver();
            }

            if (gameState != null && gameState.SessionId != 0)
            {
                await GameSaveService.SaveSessionAsync(
                    gameState.SessionId,
                    gameState,
                    isPaused: isPaused);
            }

            StateHasChanged();
        }
        catch (Exception ex)
        {
            message = $"Erreur: {ex.Message}";
        }
    }

    private async Task AttackMonster(int playerId)
    {
        if (gameState?.CurrentRoom?.Monster == null)
            return;

        var result = await GameService.AttackMonster(playerId, gameState.CurrentRoom.Monster.IdMonster);

        if (result == null || !result.Success)
        {
            message = "Erreur lors de l'attaque";
            return;
        }

        if (!result.Hit)
        {
            message = result.Message;

            if (gameState.CurrentRoom.Monster != null)
            {
                var monsterResult = await GameService.MonsterAttack(gameState.CurrentRoom.Monster.IdMonster, playerId);
                if (monsterResult != null)
                {
                    message += $"\n{monsterResult.Message}";
                }
            }
            return;
        }

        var updatedMonster = await GameService.GetMonster(gameState.CurrentRoom.Monster.IdMonster);

        if (updatedMonster.Health <= 0)
        {
            gameState.IsMonsterDefeated = true;

            int roomScore = CalculateRoomScore();
            gameState.Score += roomScore;

            message = $"{result.Message}\nVous avez vaincu {updatedMonster.Name} ! +{roomScore} points";

            await GameService.PostVictory(playerId, gameState.CurrentRoom.Id);
        }
        else
        {
            message = $"{result.Message}\n{updatedMonster.Name} a encore {updatedMonster.Health} PV.";
            gameState.CurrentRoom.Monster = updatedMonster;

            var monsterResult = await GameService.MonsterAttack(updatedMonster.IdMonster, playerId);
            if (monsterResult != null)
            {
                message += $"\n{monsterResult.Message}";
            }
        }
    }

    private async Task Defend(int playerId)
    {
        await GameService.Defend(playerId);
        message = "Vous vous mettez en position défensive.";
    }

    private async Task Heal(int playerId)
    {
        if (gameState.HealUsedInRoom >= gameState.MaxHealPerRoom)
        {
            message = "Vous n'avez plus de potions de soin pour cette salle !";
            return;
        }

        await GameService.Heal(playerId, 20);
        gameState.HealUsedInRoom++;
        message = "Vous utilisez une potion de soin. +20 PV";
    }

    private async Task Flee(int playerId)
    {
        bool success = await GameService.Flee(playerId);

        if (success)
        {
            message = "Vous fuyez la salle avec succès !";
            gameState.IsMonsterDefeated = true;
            gameState.IsRoomCompleted = true;
        }
        else
        {
            message = "Échec de la fuite ! Le monstre vous rattrape.";
            if (gameState?.CurrentRoom?.Monster != null)
            {
                var monsterResult = await GameService.MonsterAttack(gameState.CurrentRoom.Monster.IdMonster, playerId);
                if (monsterResult != null)
                {
                    message += $"\n{monsterResult.Message}";
                }
            }
        }
    }

    private async Task SearchRoom(int playerId)
    {
        var random = new Random();
        int goldFound = random.Next(10, 50) * (PlayerStats?.Level ?? 1);

        await GameService.AddGold(playerId, goldFound);

        int roomScore = CalculateRoomScore() / 2;
        gameState.Score += roomScore;

        message = $"Vous fouillez la salle et trouvez {goldFound} pièces d'or ! +{roomScore} points";
        gameState.IsRoomCompleted = true;
    }

    private async Task NextRoom()
    {
        if (gameState == null || gameState.CurrentDungeon?.Rooms == null)
            return;

        gameState.CurrentRoomIndex++;
        if (gameState.CurrentRoomIndex < gameState.TotalRooms)
        {
            gameState.CurrentRoom = gameState.CurrentDungeon.Rooms[gameState.CurrentRoomIndex];
            gameState.IsMonsterDefeated = false;
            gameState.IsRoomCompleted = false;
            gameState.HealUsedInRoom = 0;
            message = $"Vous entrez dans la salle {gameState.CurrentRoomIndex + 1}...";
        }
    }

    private async Task CompleteDungeon()
    {
        if (!AuthService.CurrentPlayerId.HasValue || gameState == null)
            return;

        var playerId = AuthService.CurrentPlayerId.Value;
        int completionBonus = CalculateDungeonBonus();
        gameState.Score += completionBonus;

        await GameService.UpdateHighScore(playerId, gameState.Score);
        await GameService.AddGameHistory(playerId, gameState.CurrentDungeon?.IdDungeon ?? 0, gameState.Score);
        await GameService.EndSession(gameState.SessionId);

        NavigationManager.NavigateTo($"/game-over?score={gameState.Score}&success=true");
    }

    private async Task GameOver()
    {
        if (!AuthService.CurrentPlayerId.HasValue || gameState == null)
            return;

        await GameService.UpdateHighScore(AuthService.CurrentPlayerId.Value, gameState.Score);
        await GameService.EndSession(gameState.SessionId);

        NavigationManager.NavigateTo($"/game-over?score={gameState.Score}&success=false");
    }

    private int CalculateRoomScore()
    {
        int baseScore = 100;
        int difficultyMultiplier = (int)gameState.DifficultyLevel + 1;
        return baseScore * difficultyMultiplier;
    }

    private int CalculateDungeonBonus()
    {
        int baseBonus = 500;
        int difficultyMultiplier = (int)gameState.DifficultyLevel + 1;
        return baseBonus * difficultyMultiplier;
    }

    private string GetRoomImage()
    {
        var roomName = gameState?.CurrentRoom?.Name;

        if (string.IsNullOrWhiteSpace(roomName))
            return "images/aventure.jpg";

        var fileName = roomName.Replace(" ", "_").ToLower();
        return $"images/{fileName}.jpg";
    }

    private string GetDifficultyName(DifficultyLevelEnum level)
    {
        return level switch
        {
            DifficultyLevelEnum.EASY => "Facile",
            DifficultyLevelEnum.MEDIUM => "Moyen",
            DifficultyLevelEnum.HARD => "Difficile",
            _ => "Inconnu",
        };
    }
}