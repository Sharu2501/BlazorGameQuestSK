namespace SharedModels.Model
{
    /// <summary>
    /// Représente l'historique, y compris les donjons complétés par un joueur.
    /// </summary>
    public class GameHistory
    {
        public int Id { get; set; }
        public Player Player { get; set; } = null!;
        public List<Dungeon> CompletedDungeons { get; set; } = new List<Dungeon>();
        public DateTime DatePlayed { get; set; } = DateTime.Now;

        public void AddDungeon(Dungeon dungeon)
        {
            CompletedDungeons.Add(dungeon);
        }

        public override string ToString()
        {
            return $"GameHistory [Id={Id}, Player={Player.Username}, DungeonsCompleted={CompletedDungeons.Count}, DatePlayed={DatePlayed}]";
        }
    }
}