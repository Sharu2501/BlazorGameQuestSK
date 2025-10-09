namespace SharedModels
{
    public class Player
    {
        public int PlayerId { get; set; }
        public int Level { get; set; }
        public string Username { get; set; }
        public int HighScore { get; set; }

        public int ExperiencePoints { get; set; }
        public int LevelCap { get; set; }
        public PlayerActionEnum Action { get; set; }
        public int Gold { get; set; }

        public Player()
        {
            PlayerId = 0;
            Level = 0;
            Username = "default";
            HighScore = 0;
        }

        public Player(int playerId, int level, string username, int highScore, 
                      PlayerActionEnum action = PlayerActionEnum.NONE, 
                      int experiencePoints = 0, int levelCap = 100)
        {
            PlayerId = playerId;
            Level = level;
            Username = username;
            HighScore = highScore;
            Action = action;
            ExperiencePoints = experiencePoints;
            LevelCap = levelCap;
        }

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

        // Getters
        public int GetPlayerId() => PlayerId;
        public int GetLevel() => Level;
        public string GetUsername() => Username;
        public int GetHighScore() => HighScore;
        public int GetExperiencePoints() => ExperiencePoints;

        public PlayerActionEnum GetAction() => Action;

        // Setters
        public void SetPlayerId(int playerId) => PlayerId = playerId;
        public void SetLevel(int level) => Level = level;
        public void SetUsername(string username) => Username = username;
        public void SetHighScore(int highScore) => HighScore = highScore;
        public void SetAction(PlayerActionEnum action) => Action = action;
    }
}