namespace SharedModels.Model
{
    /// <summary>
    /// Enumération des niveaux de difficulté pour une salle.
    /// </summary>
    public enum DifficultyLevelEnum
    {
        EASY,
        MEDIUM,
        HARD,
        EXTREME
    }

    public class Room
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = "default";
        public int Level { get; set; } = 1;
        public string Description { get; set; } = "default";
        public Monster? Monster { get; set; }
        public bool IsExplored { get; set; } = false;
        public int ExperienceGained { get; set; } = 0;
        public int GoldGained { get; set; } = 0;
        public DifficultyLevelEnum DifficultyLevel { get; set; } = DifficultyLevelEnum.EASY;
        public Player Player { get; set; } = null!;

        public void Explore()
        {
            IsExplored = true;
        }

        public override string ToString()
        {
            return $"Room[Id={Id}, Name={Name}, Level={Level}, Description={Description}, Monster={Monster}, IsExplored={IsExplored}, ExperienceGained={ExperienceGained}, GoldGained={GoldGained}, DifficultyLevel={DifficultyLevel}, Player={Player}]";
        }
    }
}