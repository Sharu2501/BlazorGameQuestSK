using SharedModels.Enum;

namespace SharedModels.Model
{
    /// <summary>
    /// Représente un monstre dans le jeu avec ses propriétés.
    /// </summary>
    public class Monster
    {
        public int IdMonster { get; set; } = 0;
        public string Name { get; set; } = "default";
        public int Level { get; set; } = 1;
        public int Health { get; set; } = 100;
        public int Attack { get; set; } = 10;
        public int Defense { get; set; } = 5;
        public MonsterTypeEnum Type { get; set; } = MonsterTypeEnum.BEAST;

        public override string ToString()
        {
            return $"Monster [IdMonster={IdMonster}, Name={Name}, Level={Level}, Health={Health}, Attack={Attack}, Defense={Defense}, Type={Type}]";
        }
    }
}