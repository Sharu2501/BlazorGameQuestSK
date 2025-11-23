public class AttackResult
{
    public bool Success { get; set; }
    public bool Hit { get; set; }
    public int Damage { get; set; }
    public int MonsterHealth { get; set; }
    public int PlayerHealth { get; set; }
    public string Message { get; set; } = string.Empty;
}