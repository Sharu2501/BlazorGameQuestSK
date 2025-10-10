namespace SharedModels
{
    public class HighScore
    {
        public int Id { get; set; }
        public Player Player { get; set; } = null!;
        public int Score { get; set; }
        public DateTime DateAchieved { get; set; } = DateTime.Now;

        public override string ToString()
        {
            return $"HighScore [Id={Id}, Player={Player.PlayerId}, Score={Score}, Date={DateAchieved}]";
        }
    }
}