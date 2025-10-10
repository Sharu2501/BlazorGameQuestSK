namespace SharedModels
{
    public class Player
    {
        public int PlayerId { get; set; } = 0;
        public int Level { get; set; } = 0;
        public string Username { get; set; } = "default";
        public int HighScore { get; set; } = 0;
        public int ExperiencePoints { get; set; } = 0;
        public int LevelCap { get; set; } = 100;
        public PlayerActionEnum Action { get; set; } = PlayerActionEnum.NONE;
        public int Gold { get; set; } = 0;

        public void ResetAction()
        {
            Action = PlayerActionEnum.NONE;
        }

        public void ChangeAction(PlayerActionEnum action)
        {
            Action = action;
        }

        public void AddExperience(int points)
        {
            ExperiencePoints += points;
            if (ExperiencePoints >= LevelCap)
            {
                LevelUp();
                ExperiencePoints -= LevelCap;
                LevelCap += 100;
            }
        }

        public void LevelUp()
        {
            Level++;
        }

        public override string ToString()
        {
            return $"Player [PlayerId={PlayerId}, \n" +
                   $"Level={Level}, Username={Username}, HighScore={HighScore}, \n" +
                   $"ExperiencePoints={ExperiencePoints}, \nLevelCap={LevelCap}, \nAction={Action}]";
        }

        public void PerformAction(PlayerActionEnum action)
        {
            var message = action switch
            {
                PlayerActionEnum.RUN => $"{Username} choisit de fuir le combat !",
                PlayerActionEnum.FIGHT => $"{Username} choisit de se battre !",
                PlayerActionEnum.HEAL => $"{Username} choisit de se soigner !",
                PlayerActionEnum.DEFEND => $"{Username} choisit de défendre !",
                PlayerActionEnum.NONE => $"{Username} n’a pas encore choisi d’action.",
                _ => $"{Username} effectue une action inconnue."
            };
            Console.WriteLine(message);
        }
    }
}