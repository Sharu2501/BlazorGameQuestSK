using BlazorGameAPI.Data;
using SharedModels.Enum;
using SharedModels.Model;

namespace BlazorGameAPI.Services
{
    public class MonsterService
    {
        private readonly ApplicationDbContext _context;

        public MonsterService(ApplicationDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Crée un nouveau monstre.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="level"></param>
        /// <param name="health"></param>
        /// <param name="attack"></param>
        /// <param name="defense"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<Monster> CreateMonster(string name, int level, int health, int attack, int defense, MonsterTypeEnum type)
        {
            var monster = new Monster
            {
                Name = name,
                Level = level,
                Health = health,
                Attack = attack,
                Defense = defense,
                Type = type
            };

            _context.Monsters.Add(monster);
            await _context.SaveChangesAsync();
            return monster;
        }
        /// <summary>
        /// Récupère un monstre par son ID.
        /// </summary>
        /// <param name="monsterId"></param>
        /// <returns></returns>
        public Task<Monster?> GetMonsterById(int monsterId)
        {
            Monster? monster = _context.Monsters.Find(monsterId);
            return Task.FromResult(monster);
        }
        /// <summary>
        /// Récupère tous les monstres.
        /// </summary>
        /// <returns></returns>
        public Task<List<Monster>> GetAllMonsters()
        {
            List<Monster> monsters = _context.Monsters.ToList();
            return Task.FromResult(monsters);
        }
        /// <summary>
        /// Récupère les monstres dans une plage de niveaux spécifiée.
        /// </summary>
        /// <param name="minLevel"></param>
        /// <param name="maxLevel"></param>
        /// <returns></returns>
        public Task<List<Monster>> GetMonstersByLevelRange(int minLevel, int maxLevel)
        {
            List<Monster> monsters = _context.Monsters
                .Where(m => m.Level >= minLevel && m.Level <= maxLevel)
                .ToList();
            return Task.FromResult(monsters);
        }
        /// <summary>
        /// Récupère un monstre aléatoire dans une plage de niveaux spécifiée.
        /// </summary>
        /// <param name="minLevel"></param>
        /// <param name="maxLevel"></param>
        /// <returns></returns>
        public Task<Monster?> GetRandomMonster(int minLevel, int maxLevel)
        {
            var monstersInRange = _context.Monsters
                .Where(m => m.Level >= minLevel && m.Level <= maxLevel)
                .ToList();

            if (monstersInRange.Count == 0)
            {
                return Task.FromResult<Monster?>(null);
            }

            var random = new Random();
            int index = random.Next(monstersInRange.Count);
            return Task.FromResult<Monster?>(monstersInRange[index]);
        }
        /// <summary>
        /// Met à jour les informations d'un monstre.
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        public Task<bool> UpdateMonster(Monster monster)
        {
            Monster? existingMonster = _context.Monsters.Find(monster.IdMonster);
            if (existingMonster == null)
            {
                return Task.FromResult(false);
            }

            existingMonster.Name = monster.Name;
            existingMonster.Level = monster.Level;
            existingMonster.Health = monster.Health;
            existingMonster.Attack = monster.Attack;
            existingMonster.Defense = monster.Defense;
            existingMonster.Type = monster.Type;

            _context.SaveChanges();
            return Task.FromResult(true);
        }
        /// <summary>
        /// Supprime un monstre par son ID.
        /// </summary>
        /// <param name="monsterId"></param>
        /// <returns></returns>
        public Task<bool> DeleteMonster(int monsterId)
        {
            Monster? monster = _context.Monsters.Find(monsterId);
            if (monster == null)
            {
                return Task.FromResult(false);
            }

            _context.Monsters.Remove(monster);
            _context.SaveChanges();
            return Task.FromResult(true);
        }
        /// <summary>
        /// Génère un monstre adapté au niveau du joueur et à la difficulté spécifiée.
        /// </summary>
        /// <param name="playerLevel"></param>
        /// <param name="difficulty"></param>
        /// <returns></returns>
        public async Task<Monster> GenerateMonsterForLevel(int playerLevel, DifficultyLevelEnum difficulty)
        {
            var random = new Random();

            var levelVariation = difficulty switch
            {
                DifficultyLevelEnum.EASY => random.Next(-2, 1),
                DifficultyLevelEnum.MEDIUM => random.Next(-1, 2),
                DifficultyLevelEnum.HARD => random.Next(0, 3),
                DifficultyLevelEnum.EXTREME => random.Next(1, 5),
                _ => 0
            };

            var monsterLevel = Math.Max(1, playerLevel + levelVariation);

            var monsterType = (MonsterTypeEnum)random.Next(0, Enum.GetValues(typeof(MonsterTypeEnum)).Length);

            var monsterNames = monsterType switch
            {
                MonsterTypeEnum.DRAGON => new string[]
                {
                    "Drakor",
                    "Fyrezor",
                    "Écaille-Ailes",
                    "Infernus"
                },
                MonsterTypeEnum.GOBLIN => new string[]
                {
                    "Griblou",
                    "Snark",
                    "Grimp",
                    "Razz"
                },
                MonsterTypeEnum.TROLL => new string[]
                {
                    "Gronk",
                    "Toud",
                    "Bouldar",
                    "Fracasse"
                },
                MonsterTypeEnum.UNDEAD => new string[]
                {
                    "Guerrier Squelette",
                    "Zombie",
                    "Apparition",
                    "Goule"
                },
                MonsterTypeEnum.BEAST => new string[]
                {
                    "Grand Loup",
                    "Araignée Géante",
                    "Ours des Cavernes",
                    "Vouivre"
                },
                MonsterTypeEnum.DEMON => new string[]
                {
                    "Engeance des Enfers",
                    "Seigneur de l’Effroi",
                    "Abyssal",
                    "Tourmenteur"
                },
                MonsterTypeEnum.ELEMENTAL => new string[]
                {
                    "Élémentaire de Feu",
                    "Golem de Glace",
                    "Esprit de Tempête",
                    "Gardien de Terre"
                },
                MonsterTypeEnum.HUMANOID => new string[]
                {
                    "Bandit",
                    "Cultiste",
                    "Chevalier Noir",
                    "Assassin"
                },
                _ => new string[] { "Monstre" }
            };

            var name = monsterNames[random.Next(monsterNames.Length)];

            var baseHealth = 50 + (monsterLevel * 15);
            var baseAttack = 5 + (monsterLevel * 2);
            var baseDefense = 3 + (monsterLevel * 1);

            var health = baseHealth + random.Next(-baseHealth / 10, baseHealth / 10);
            var attack = baseAttack + random.Next(-baseAttack / 10, baseAttack / 10);
            var defense = baseDefense + random.Next(-baseDefense / 10, baseDefense / 10);

            var monster = new Monster
            {
                Name = name,
                Level = monsterLevel,
                Health = health,
                Attack = attack,
                Defense = defense,
                Type = monsterType
            };

            _context.Monsters.Add(monster);
            await _context.SaveChangesAsync();

            return monster;
        }
    }
}