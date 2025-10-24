namespace SharedModels.Model
{
    /// <summary>
    /// Représente un donjon dans le jeu avec ses propriétés.
    /// </summary>
    public class Dungeon
    {
        public int IdDungeon { get; set; } = 0;
        public string Name { get; set; } = "default";
        public string Description { get; set; } = "default";
        public List<Room> Rooms { get; set; } = new List<Room>();
        public bool IsExplored { get; set; }
        public Artifact? Artifact { get; set; }

        public override string ToString()
        {
            return $"Dungeon [IdDungeon={IdDungeon}, Name={Name}, Description={Description}, RoomsCount={Rooms.Count}]";
        }
        
    
    }
}