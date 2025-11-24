using SharedModels.Enum;

namespace SharedModels.Model
{
    /// <summary>
    /// Représente une session de jeu en cours pour un joueur.
    /// </summary>
    public class GameState
    {
         public Dungeon? CurrentDungeon { get; set; }
        public Room? CurrentRoom { get; set; }
        public int CurrentRoomIndex { get; set; }
        public int TotalRooms { get; set; }
        public bool IsMonsterDefeated { get; set; }
        public bool IsRoomCompleted { get; set; }
        public int Score { get; set; }
        public int SessionId { get; set; }
        public int HealUsedInRoom { get; set; }
        public int MaxHealPerRoom { get; set; } = 2;
        public DifficultyLevelEnum DifficultyLevel { get; set; }
       public DateTime StartTime { get; set; } = DateTime.UtcNow;

        public override string ToString()
        {
            return $"GameSession(SessionId={SessionId}, CurrentDungeon={CurrentDungeon?.Name}, CurrentRoom={CurrentRoom?.Name}, CurrentRoomIndex={CurrentRoomIndex}, TotalRooms={TotalRooms}, IsMonsterDefeated={IsMonsterDefeated}, IsRoomCompleted={IsRoomCompleted}, Score={Score}, HealUsedInRoom={HealUsedInRoom}, MaxHealPerRoom={MaxHealPerRoom}, DifficultyLevel={DifficultyLevel}, StartTime={StartTime})";
        }
    }
}