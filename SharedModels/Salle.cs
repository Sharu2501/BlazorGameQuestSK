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
        public int Id { get; set; }
        public string Nom { get; set; } = null!;
        public int Niveau { get; set; }
        public string Description { get; set; } = null!;
        public Monster? Monstre { get; set; }
        public bool EstExploree { get; set; }
        public int ExperienceGagnee { get; set; }
        public int OrGagne { get; set; }
        public NiveauDifficulteEnum NiveauDifficulte { get; set; }
        public Joueur Joueur { get; set; } = null!;

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

        public Salle(int id, string nom, int niveau, string description, Monster? monstre,
                     bool estExploree, int experienceGagnee, int orGagne,
                     NiveauDifficulteEnum niveauDifficulte, Joueur joueur)
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