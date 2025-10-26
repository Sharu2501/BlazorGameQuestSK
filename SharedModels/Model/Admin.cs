namespace SharedModels.Model
{
    public class Admin : User
    {
        public int AdminId { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime LastLoginDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
        
    }
}