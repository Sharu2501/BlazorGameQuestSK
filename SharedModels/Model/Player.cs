using SharedModels.Enum;

namespace SharedModels.Model
{
    /// <summary>
    /// Représente un joueur dans le jeu avec ses propriétés et ses actions. 
    /// </summary>
    public class Player : User
    {
        public string ExternalId { get; set; } = "";
        public int Level { get; set; } = 0;
        public HighScore HighScore { get; set; } = new HighScore();
        public int ExperiencePoints { get; set; } = 0;
        public int LevelCap { get; set; } = 100;
        public PlayerActionEnum Action { get; set; } = PlayerActionEnum.NONE;
        public int Gold { get; set; } = 0;
        public int Health { get; set; } = 100;
        public int MaxHealth { get; set; } = 100;
        public int Attack { get; set; } = 10;
        public int Defense { get; set; } = 5;
        public List<Artifact> Inventory { get; set; } = new List<Artifact>();   
        public List<GameHistory> GameHistories { get; set; } = new();
        public bool IsActive { get; set; } = true;
    }
}