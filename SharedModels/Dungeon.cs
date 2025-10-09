namespace SharedModels
{
    public class Dungeon
    {
        public int IdDungeon { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Room> Rooms { get; set; }
        public bool IsExplored { get; set; }
        public Artifact? Artifact { get; set; }

        public Dungeon()
        {
            IdDungeon = 0;
            Name = "default";
            Description = "default";
            Rooms = new List<Room>();
        }

        public Dungeon(int idDungeon, string name, string description, List<Room> rooms)
        {
            IdDungeon = idDungeon;
            Name = name;
            Description = description;
            Rooms = rooms;
        }

        public override string ToString()
        {
            return $"Dungeon [IdDungeon={IdDungeon}, Name={Name}, Description={Description}, RoomsCount={Rooms.Count}]";
        }
        public void ExploreDungeon()
        {
            IsExplored = true;
        }
    }
}