namespace SharedModels
{
    public class Donjon
    {
        public int IdDonjon { get; set; }
        public string Nom { get; set; }
        public string Description { get; set; }
        public List<Salle> Salles { get; set; }
        public bool Explorer { get; set; }
        public Artefact? Artefact { get; set; }

        public Donjon()
        {
            IdDonjon = 0;
            Nom = "default";
            Description = "default";
            Salles = new List<Salle>();
        }

        public Donjon(int idDonjon, string nom, string description, List<Salle> salles)
        {
            IdDonjon = idDonjon;
            Nom = nom;
            Description = description;
            Salles = salles;
        }

        public override string ToString()
        {
            return $"Donjon [IdDonjon={IdDonjon}, Nom={Nom}, Description={Description}, SallesCount={Salles.Count}]";
        }
        public void ExplorerDonjon()
        {
            Explorer = true;
        }
    }
}