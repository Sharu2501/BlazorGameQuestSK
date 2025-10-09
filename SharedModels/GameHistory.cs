namespace SharedModels
{
    public class GameHistory
    {
        public int Id { get; set; }
        public Player Player { get; set; } = null!;
        public List<Dungeon> CompletedDungeons { get; set; } = new List<Dungeon>();
        public DateTime DatePlayed { get; set; }

        public GameHistory()
        {
            DatePlayed = DateTime.Now;
        }

        public GameHistory(int id, Player player, List<Dungeon> completedDungeons)
        {
            Id = id;
            Player = player;
            CompletedDungeons = completedDungeons;
            DatePlayed = DateTime.Now;
        }

        public void AddDungeon(Dungeon dungeon)
        {
            CompletedDungeons.Add(dungeon);
        }

        public override string ToString()
        {
            return $"GameHistory [Id={Id}, Player={Player.GetUsername()}, DungeonsCompleted={CompletedDungeons.Count}, DatePlayed={DatePlayed}]";
        }
    }
}