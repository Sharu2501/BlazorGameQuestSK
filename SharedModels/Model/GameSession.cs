namespace SharedModels.Model
{
    public class GameSession
    {
        public int SessionId { get; set; } = 0;
        public int PlayerId { get; set; } = 0;
        public int CurrentRoomId { get; set; } = 0;
        public int CurrentDungeonId { get; set; } = 0;
        public bool IsActive { get; set; } = true;
       public DateTime StartTime { get; set; } = DateTime.Now;

        public override string ToString()
        {
            return $"GameSession[SessionId={SessionId}, PlayerId={PlayerId}, CurrentRoomId={CurrentRoomId}, CurrentDungeonId={CurrentDungeonId}, IsActive={IsActive}, StartTime={StartTime}]";
        }
    }
}