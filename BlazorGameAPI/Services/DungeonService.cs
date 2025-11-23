using BlazorGameAPI.Data;
using SharedModels.Model;
using Microsoft.EntityFrameworkCore;
using BlazorGameAPI.Services;
using SharedModels.Enum;
namespace BlazorGameAPI.Services
{
    public class DungeonService
    {
        private readonly ApplicationDbContext _context;
        private readonly RoomService _roomService;
        private readonly ArtifactService _artifactService;
        public DungeonService(
            ApplicationDbContext context,
            RoomService roomService,
            ArtifactService artifactService)
        {
            _context = context;
            _roomService = roomService;
            _artifactService = artifactService;
        }
        /// <summary>
        /// Crée un nouveau donjon.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public Task<Dungeon> CreateDungeon(string name, string description)
        {
            var dungeon = new Dungeon
            {
                Name = name,
                Description = description
            };

            _context.Dungeons.Add(dungeon);
            _context.SaveChanges();

            return Task.FromResult(dungeon);
        }
        /// <summary>
        /// Récupère un donjon par son ID.
        /// </summary>
        /// <param name="dungeonId"></param>
        /// <returns></returns>
        public Task<Dungeon?> GetDungeonById(int dungeonId)
        {
            Dungeon? dungeon = _context.Dungeons.Find(dungeonId);
            return Task.FromResult(dungeon);
        }
        /// <summary>
        /// Récupère tous les donjons.
        /// </summary>
        /// <returns></returns>
        public Task<List<Dungeon>> GetAllDungeons()
        {
            List<Dungeon> dungeons = _context.Dungeons.ToList();
            return Task.FromResult(dungeons);
        }
        /// <summary>
        /// Récupère les donjons par statut d'exploration.
        /// </summary>
        /// <param name="isExplored"></param>
        /// <returns></returns>
        public Task<List<Dungeon>> GetDungeonsByExplorationStatus(bool isExplored)
        {
            List<Dungeon> dungeons = _context.Dungeons
                .Where(d => d.IsExplored == isExplored)
                .ToList();
            return Task.FromResult(dungeons);
        }
        /// <summary>
        /// Ajoute une salle à un donjon.     
        /// </summary>
        /// <param name="dungeonId"></param>
        /// <param name="room"></param>
        /// <returns></returns>
        public Task<bool> AddRoomToDungeon(int dungeonId, Room room)
        {
            Dungeon? dungeon = _context.Dungeons.Find(dungeonId);
            if (dungeon == null)
            {
                return Task.FromResult(false);
            }

            dungeon.Rooms.Add(room);
            _context.SaveChanges();

            return Task.FromResult(true);
        }
        /// <summary>
        /// Assigne un artefact à un donjon.
        /// </summary>
        /// <param name="dungeonId"></param>
        /// <param name="artifactId"></param>
        /// <returns></returns>
        public Task<bool> AssignArtifactToDungeon(int dungeonId, int artifactId)
        {
            Dungeon? dungeon = _context.Dungeons.Find(dungeonId);
            Artifact? artifact = _context.Artifacts.Find(artifactId);

            if (dungeon == null || artifact == null)
            {
                return Task.FromResult(false);
            }

            dungeon.Artifact = artifact;
            _context.SaveChanges();

            return Task.FromResult(true);
        }
        /// <summary>
        /// Marque un donjon comme exploré.
        /// </summary>
        /// <param name="dungeonId"></param>
        /// <returns></returns>
        public Task<bool> MarkDungeonAsExplored(int dungeonId)
        {
            Dungeon? dungeon = _context.Dungeons.Find(dungeonId);
            if (dungeon == null)
            {
                return Task.FromResult(false);
            }

            dungeon.IsExplored = true;
            _context.SaveChanges();

            return Task.FromResult(true);
        }
        /// <summary>
        /// Vérifie si un donjon est complété.
        /// </summary>
        /// <param name="dungeonId"></param>
        /// <returns></returns>
        public Task<bool> IsDungeonCompleted(int dungeonId)
        {
            Dungeon? dungeon = _context.Dungeons.Find(dungeonId);
            return Task.FromResult(dungeon?.IsExplored ?? false);
        }
        /// <summary>
        /// Récupère le pourcentage de progression d'un donjon.
        /// </summary>
        /// <param name="dungeonId"></param>
        /// <returns></returns>
        public async Task<int> GetDungeonProgress(int dungeonId)
        {
            var dungeon = await _context.Dungeons
                .Include(d => d.Rooms)
                .FirstOrDefaultAsync(d => d.IdDungeon == dungeonId);

            if (dungeon == null || dungeon.Rooms.Count == 0)
            {
                return 0;
            }

            var totalRooms = dungeon.Rooms.Count;
            var exploredRooms = dungeon.Rooms.Count(r => r.IsExplored);

            var progress = (int)((exploredRooms / (double)totalRooms) * 100);

            return progress;
        }
        /// <summary>
        /// Génère un donjon aléatoire avec des salles et éventuellement un artefact.
        /// </summary>
        /// <param name="numberOfRooms"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public async Task<Dungeon> GenerateRandomDungeon(int numberOfRooms, int level)
        {
            var random = new Random();

            Dictionary<string, string> dungeonTemplates = new()
            {
                { "The Forsaken Depths", "An ancient place filled with untold dangers and treasures" },
                { "Tower of Shadows", "Few who enter these halls ever return to tell the tale" },
                { "Crimson Catacombs", "Dark magic permeates every stone of this cursed place" },
                { "The Lost Temple", "Legends speak of great riches hidden within" },
                { "Abyssal Fortress", "The air itself seems hostile in this forsaken realm" },
                { "Dragon's Keep", "A maze of corridors where death lurks around every corner" },
                { "The Cursed Citadel", "Ancient curses protect the treasures within" },
                { "Ruins of Eternity", "Time has forgotten this place, but evil has not" },
                { "The Dark Sanctum", "A sanctuary corrupted by dark forces" },
                { "Tomb of the Ancient Kings", "The resting place of forgotten rulers" },
                { "The Infernal Dungeon", "Flames and fury await those who dare enter" },
                { "Castle Dreadmoor", "A once-proud fortress now home to nightmares" }
            };

            var selectedDungeon = dungeonTemplates.ElementAt(random.Next(dungeonTemplates.Count));
            var name = selectedDungeon.Key;
            var description = selectedDungeon.Value;

            var dungeon = new Dungeon
            {
                Name = name,
                Description = description,
                IsExplored = false,
                Rooms = new List<Room>()
            };

            _context.Dungeons.Add(dungeon);
            await _context.SaveChangesAsync();

            var difficulties = Enum.GetValues<DifficultyLevelEnum>();

            for (int i = 0; i < numberOfRooms; i++)
            {
                DifficultyLevelEnum difficulty;
                if (i < numberOfRooms * 0.3)
                    difficulty = DifficultyLevelEnum.EASY;
                else if (i < numberOfRooms * 0.6)
                    difficulty = DifficultyLevelEnum.MEDIUM;
                else if (i < numberOfRooms * 0.85)
                    difficulty = DifficultyLevelEnum.HARD;
                else
                    difficulty = DifficultyLevelEnum.EXTREME;

                var room = await _roomService.GenerateRoom(level, difficulty);
                dungeon.Rooms.Add(room);
            }

            if (random.Next(100) < 50)
            {
                var artifactNames = new Dictionary<RarityEnum, string[]>
                {
                    { RarityEnum.COMMON, new[] { "Rusty Sword", "Worn Shield", "Leather Boots" } },
                    { RarityEnum.RARE, new[] { "Silver Dagger", "Enchanted Ring", "Magic Cloak" } },
                    { RarityEnum.EPIC, new[] { "Dragon Slayer", "Crown of Kings", "Phoenix Feather" } },
                    { RarityEnum.LEGENDARY, new[] { "Excalibur", "Thor's Hammer", "Holy Grail" } },
                    { RarityEnum.MYTHICAL, new[] { "Eye of Eternity", "Void Essence", "Celestial Sword" } }
                };

                var rarities = new[] { RarityEnum.COMMON, RarityEnum.RARE, RarityEnum.EPIC, RarityEnum.LEGENDARY, RarityEnum.MYTHICAL };
                var weights = new[] { 50, 30, 15, 4, 1 };
                var totalWeight = weights.Sum();
                var randomValue = random.Next(totalWeight);

                RarityEnum selectedRarity = RarityEnum.COMMON;
                int cumulative = 0;
                for (int i = 0; i < weights.Length; i++)
                {
                    cumulative += weights[i];
                    if (randomValue < cumulative)
                    {
                        selectedRarity = rarities[i];
                        break;
                    }
                }

                var names = artifactNames[selectedRarity];
                var artifactName = names[random.Next(names.Length)];
                var artifactDescription = $"A {selectedRarity.ToString().ToLower()} artifact found in the depths of the dungeon";

                var newArtifact = new Artifact
                {
                    Name = artifactName,
                    Description = artifactDescription,
                    Rarity = selectedRarity
                };

                _context.Artifacts.Add(newArtifact);
                await _context.SaveChangesAsync();

                dungeon.Artifact = newArtifact;
            }

            await _context.SaveChangesAsync();

            return dungeon;
        }
        /// <summary>
        /// Met à jour les informations d'un donjon.
        /// </summary>
        /// <param name="dungeon"></param>
        /// <returns></returns>
        public Task<bool> UpdateDungeon(Dungeon dungeon)
        {
            var existingDungeon = _context.Dungeons.Find(dungeon.IdDungeon);
            if (existingDungeon == null)
            {
                return Task.FromResult(false);
            }

            existingDungeon.Name = dungeon.Name;
            existingDungeon.Description = dungeon.Description;
            existingDungeon.IsExplored = dungeon.IsExplored;

            _context.SaveChanges();
            return Task.FromResult(true);
        }
        /// <summary>
        /// Supprime un donjon par son ID.
        /// </summary>
        /// <param name="dungeonId"></param>
        /// <returns></returns>
        public Task<bool> DeleteDungeon(int dungeonId)
        {
            var dungeon = _context.Dungeons.Find(dungeonId);
            if (dungeon == null)
            {
                return Task.FromResult(false);
            }

            _context.Dungeons.Remove(dungeon);
            _context.SaveChanges();
            return Task.FromResult(true);
        }
    }
}