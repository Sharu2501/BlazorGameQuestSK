namespace SharedModels.Model.DTOs
{
    public class SaveSessionDto
    {
        public string StateJson { get; set; } = string.Empty;
        public bool IsPaused { get; set; }
    }
}