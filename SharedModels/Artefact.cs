namespace SharedModels
{
    public class Artefact
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Description { get; set; }
        public RareteEnum Rarete { get; set; }
        public Artefact()
        {
            Id = 0;
            Nom = "default";
            Description = "default";
            Rarete = RareteEnum.COMMUN;
        }
        public Artefact(int id, string nom, string description, RareteEnum rarete)
        {
            Id = id;
            Nom = nom;
            Description = description;
            Rarete = rarete;
        }
    }
}