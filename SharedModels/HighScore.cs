namespace SharedModels
{
    public class HighScore
    {
        public int Id { get; set; }
        public Player Player { get; set; } = null!;
        public int Score { get; set; }
        public DateTime DateAchieved { get; set; }

        public HighScore()
        {
            DateAchieved = DateTime.Now;
        }

        public HighScore(int id, Player player, int score)
        {
            Id = id;
            Player = player;
            Score = score;
            DateAchieved = DateTime.Now;
        }

        public override string ToString()
        {
            return $"HighScore [Id={Id}, Player={Player.GetUsername()}, Score={Score}, Date={DateAchieved}]";
        }
    }
}