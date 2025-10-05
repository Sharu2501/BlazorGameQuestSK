namespace SharedModels
{
    public class Joueur
    {
        private int IdJoueur { get; set; }
        private int Level { get; set; }
        private string Pseudo { get; set; }
        private int HighScore { get; set; }

        private int ExperiencePoints { get; set; }
        private int LevelCap { get; set; }
        private JoueurActionEnum Action { get; set; }
        private int Or { get; set; }

        public Joueur()
        {
            IdJoueur = 0;
            Level = 0;
            Pseudo = "default";
            HighScore = 0;
        }

        public Joueur(int idJoueur, int level, string pseudo, int highScore, JoueurActionEnum action = JoueurActionEnum.NONE, int ExperiencePoints = 0, int LevelCap = 100)
        {
            IdJoueur = idJoueur;
            Level = level;
            Pseudo = pseudo;
            HighScore = highScore;
            Action = action;
            this.ExperiencePoints = ExperiencePoints;
            this.LevelCap = LevelCap;
        }

        public void ResetAction()
        {
            Action = JoueurActionEnum.NONE;
        }
        public void ChangeAction(JoueurActionEnum action)
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
            return $"Joueur [IdJoueur={IdJoueur}, \n" +
            $"Level ={Level}, Pseudo={Pseudo}, HighScore={HighScore}, \n" +
            $"ExperiencePoints ={ExperiencePoints}, \n LevelCap={LevelCap}, \nAction={Action}]";
        }

        public void PerformAction(JoueurActionEnum action)
        {
            var message = action switch
            {
                JoueurActionEnum.FUIR => $"{Pseudo} choisit de fuir le combat !",
                JoueurActionEnum.COMBATTRE => $"{Pseudo} choisit de combattre !",
                JoueurActionEnum.SOIGNER => $"{Pseudo} choisit de se soigner !",
                JoueurActionEnum.DEFENDRE => $"{Pseudo} choisit de se défendre !",
                JoueurActionEnum.NONE => $"{Pseudo} n'a pas encore choisi d'action.",
                _ => $"{Pseudo} effectue une action inconnue."
            };
            Console.WriteLine(message);
        }
        // Getters
        public int GetIdJoueur() { return IdJoueur; }
        public int GetLevel() { return Level; }
        public string GetPseudo() { return Pseudo; }
        public int GetHighScore() { return HighScore; }
        public JoueurActionEnum GetAction() { return Action; }

        // Setters
        public void SetIdJoueur(int idJoueur) { IdJoueur = idJoueur; }
        public void SetLevel(int level) { Level = level; }
        public void SetPseudo(string pseudo) { Pseudo = pseudo; }
        public void SetHighScore(int highScore) { HighScore = highScore; }
        public void SetAction(JoueurActionEnum action) { Action = action; }
    }
}