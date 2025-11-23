namespace SharedModels.Model
{
    /// <summary>
    /// Représente un administrateur du système.
    /// </summary>
    public class Admin : User
    {
        public int AdminId { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastLoginDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        
    }
}