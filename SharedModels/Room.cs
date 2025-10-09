namespace SharedModels
{
    public enum DifficultyLevelEnum
    {
        EASY,
        MEDIUM,
        HARD,
        EXTREME
    }

    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Level { get; set; }
        public string Description { get; set; } = null!;
        public Monster? Monster { get; set; }
        public bool IsExplored { get; set; }
        public int ExperienceGained { get; set; }
        public int GoldGained { get; set; }
        public DifficultyLevelEnum DifficultyLevel { get; set; }
        public Player Player { get; set; } = null!;

        public Room()
        {
            Id = 0;
            Name = "default";
            Level = 1;
            Description = "default";
            Monster = null;
            Player = null!;
            IsExplored = false;
            ExperienceGained = 0;
            GoldGained = 0;
            DifficultyLevel = DifficultyLevelEnum.EASY;
        }

        public Room(int id, string name, int level, string description, Monster? monster,
                    bool isExplored, int experienceGained, int goldGained,
                    DifficultyLevelEnum difficultyLevel, Player player)
        {
            Id = id;
            Name = name;
            Level = level;
            Description = description;
            Monster = monster;
            IsExplored = isExplored;
            ExperienceGained = experienceGained;
            GoldGained = goldGained;
            DifficultyLevel = difficultyLevel;
            Player = player;
        }

        public Room(Player player)
        {
            Id = 0;
            Name = "default";
            Level = 1;
            Description = "default";
            Monster = null;
            Player = player;
            IsExplored = false;
            ExperienceGained = 0;
            GoldGained = 0;
            DifficultyLevel = DifficultyLevelEnum.EASY;
        }

        public void Explore()
        {
            IsExplored = true;
        }

        public override string ToString()
        {
            return $"Room [Id={Id}, Name={Name}, Level={Level}, Description={Description}, Monster={Monster}, IsExplored={IsExplored}, ExperienceGained={ExperienceGained}, GoldGained={GoldGained}, DifficultyLevel={DifficultyLevel}, Player={Player}]";
        }
    }
}