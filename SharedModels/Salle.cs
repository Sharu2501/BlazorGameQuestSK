namespace SharedModels
{
    public enum NiveauDifficulteEnum
    {
        FACILE,
        MOYEN,
        DIFFICILE,
        EXTREME
    }
    public class Salle
    {
        private int Id { get; set; }
        private string Nom { get; set; } = null!;
        private int Niveau { get; set; }
        private string Description { get; set; } = null!;
        private Monster? Monstre { get; set; }
        private bool EstExploree { get; set; }
        private int ExperienceGagnee { get; set; }
        private int OrGagne { get; set; }
        private NiveauDifficulteEnum NiveauDifficulte { get; set; }
        private Joueur Joueur { get; set; } = null!;
        public Salle()
        {
            Id = 0;
            Nom = "default";
            Niveau = 1;
            Description = "default";
            Monstre = null;
            Joueur = null!;
            EstExploree = false;
            ExperienceGagnee = 0;
            OrGagne = 0;
            NiveauDifficulte = NiveauDifficulteEnum.FACILE;
        }
        public Salle(int id, string nom, int niveau, string description, Monster? monstre, bool estExploree, int experienceGagnee, int orGagne, NiveauDifficulteEnum niveauDifficulte, Joueur joueur)
        {
            Id = id;
            Nom = nom;
            Niveau = niveau;
            Description = description;
            Monstre = monstre;
            EstExploree = estExploree;
            ExperienceGagnee = experienceGagnee;
            OrGagne = orGagne;
            NiveauDifficulte = niveauDifficulte;
            Joueur = joueur;
        }
        public void Explorer()
        {
            EstExploree = true;
        }
        public override string ToString()
        {
            return $"Salle [Id={Id}, Nom={Nom}, Niveau={Niveau}, Description={Description}, Monstre={Monstre}, EstExploree={EstExploree}, ExperienceGagnee={ExperienceGagnee}, OrGagne={OrGagne}, NiveauDifficulte={NiveauDifficulte}, Joueur={Joueur}]";
        }
    }
}