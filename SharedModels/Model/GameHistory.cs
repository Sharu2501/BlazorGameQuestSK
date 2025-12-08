using System.Text.Json.Serialization;

namespace SharedModels.Model
{
    /// <summary>
    /// Représente l'historique, y compris les donjons complétés par un joueur.
    /// </summary>
    public class GameHistory
    {
        public int Id { get; set; }
        [JsonIgnore]  
        public Player Player { get; set; } = null!;
        public int Score { get; set; } = 0;
        public List<Dungeon> CompletedDungeons { get; set; } = new List<Dungeon>();
        public DateTime DatePlayed { get; set; } = DateTime.UtcNow;

        public override string ToString()
        {
            return $"GameHistory [Id={Id}, Player={Player.Username}, DungeonsCompleted={CompletedDungeons.Count}, DatePlayed={DatePlayed}]";
        }
    }
}