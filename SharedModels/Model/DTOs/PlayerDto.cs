using SharedModels.Enum;

namespace SharedModels.Model.DTOs
{
    /// <summary>
    /// DTO pour transférer les statistiques complètes d'un joueur
    /// </summary>
    public class PlayerStatsDto
    {
        public int PlayerId { get; set; }
        public string Username { get; set; } = string.Empty;
        public int Level { get; set; }
        public int ExperiencePoints { get; set; }
        public int LevelCap { get; set; }
        public double ExperiencePercentage { get; set; }
        public int Gold { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public double HealthPercentage { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public PlayerActionEnum CurrentAction { get; set; }
        public int InventoryCount { get; set; }
        public int HighestScore { get; set; }
        public DateTime? HighScoreDate { get; set; }
        public int TotalDungeonsCompleted { get; set; }
    }
}