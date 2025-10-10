namespace SharedModels
{
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
        public void ExploreDungeon()
        {
            IsExplored = true;
        }
    }
}