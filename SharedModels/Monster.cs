namespace SharedModels
{
    public class Monster
    {
        public int IdMonster { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public int Health { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public MonsterTypeEnum Type { get; set; }

        public Monster()
        {
            IdMonster = 0;
            Name = "default";
            Level = 1;
            Health = 100;
            Attack = 10;
            Defense = 5;
            Type = MonsterTypeEnum.BEAST;
        }

        public Monster(int idMonster, string name, int level, int health, int attack, int defense, MonsterTypeEnum type)
        {
            IdMonster = idMonster;
            Name = name;
            Level = level;
            Health = health;
            Attack = attack;
            Defense = defense;
            Type = type;
        }

        public override string ToString()
        {
            return $"Monster [IdMonster={IdMonster}, Name={Name}, Level={Level}, Health={Health}, Attack={Attack}, Defense={Defense}, Type={Type}]";
        }
    }
}