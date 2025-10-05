namespace SharedModels
{
    public class Salle
    {
        public int Id { get; set; }
        public string Nom { get; set; } = null!;
        public int Niveau { get; set; }
        public string Description { get; set; } = null!;
    }
}