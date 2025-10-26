using BlazorGameAPI.Data;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SharedModels.Enum;
using SharedModels.Model;

namespace BlazorGameAPI.Services
{
    public class RoomService
    {
        private readonly ApplicationDbContext _context;
        private readonly MonsterService _monsterService;

        public RoomService(ApplicationDbContext context, MonsterService monsterService)
        {
            _context = context;
            _monsterService = monsterService;
        }

        /// <summary>
        /// Crée une nouvelle salle.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="level"></param>
        /// <param name="description"></param>
        /// <param name="difficulty"></param>
        /// <param name="experienceGained"></param>
        /// <param name="goldGained"></param>
        /// <returns></returns>
        public Task<Room> CreateRoom(string name, int level, string description, DifficultyLevelEnum difficulty, int experienceGained, int goldGained)
        {
            Room? room = new Room
            {
                Name = name,
                Level = level,
                Description = description,
                DifficultyLevel = difficulty,
                ExperienceGained = experienceGained,
                GoldGained = goldGained
            };

            _context.Rooms.Add(room);
            _context.SaveChanges();

            return Task.FromResult(room);
        }

        /// <summary>
        /// Récupère une salle par son ID.
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        public Task<Room?> GetRoomById(int roomId)
        {
            Room? room = _context.Rooms.Find(roomId);
            return Task.FromResult(room);
        }

        public Task<List<Room>> GetRoomsByDungeonId(int dungeonId)
        {
            List<Room> rooms = _context.Rooms
                .Where(r => r.Id == dungeonId)
                .ToList();
            return Task.FromResult(rooms);
        }

        public Task<bool> AssignMonsterToRoom(int roomId, int monsterId)
        {
            var room = _context.Rooms.Find(roomId);
            var monster = _context.Monsters.Find(monsterId);

            if (room == null || monster == null)
            {
                return Task.FromResult(false);
            }

            room.Monster = monster;
            _context.SaveChanges();

            return Task.FromResult(true);
        }

        public Task<bool> MarkRoomAsExplored(int roomId)
        {
            var room = _context.Rooms.Find(roomId);

            if (room == null)
            {
                return Task.FromResult(false);
            }

            room.IsExplored = true;
            _context.SaveChanges();

            return Task.FromResult(true);
        }

        public Task<bool> IsRoomExplored(int roomId)
        {
            var room = _context.Rooms.Find(roomId);
            return Task.FromResult(room?.IsExplored ?? false);
        }

        public Task<bool> UpdateRoom(Room room)
        {
            var existingRoom = _context.Rooms.Find(room.Id);
            if (existingRoom == null)
            {
                return Task.FromResult(false);
            }

            existingRoom.Name = room.Name;
            existingRoom.Level = room.Level;
            existingRoom.Description = room.Description;
            existingRoom.Monster = room.Monster;
            existingRoom.IsExplored = room.IsExplored;
            existingRoom.ExperienceGained = room.ExperienceGained;
            existingRoom.GoldGained = room.GoldGained;
            existingRoom.DifficultyLevel = room.DifficultyLevel;

            _context.SaveChanges();
            return Task.FromResult(true);
        }
        public Task<bool> DeleteRoom(int roomId)
        {
            var room = _context.Rooms.Find(roomId);
            if (room == null)
            {
                return Task.FromResult(false);
            }

            _context.Rooms.Remove(room);
            _context.SaveChanges();
            return Task.FromResult(true);
        }

        /// <summary>
        /// Génère une salle aléatoire pour un donjon donné.
        /// </summary>
        /// <param name="dungeonLevel"></param>
        /// <param name="difficulty"></param>
        /// <returns></returns>
        public async Task<Room> GenerateRoom(int dungeonLevel, DifficultyLevelEnum difficulty)
        {
            var random = new Random();

            // Dictionnaire nom → description
            Dictionary<string, string> roomTemplates = new()
    {
        { "Dark Chamber", "A dark passage filled with ancient mysteries" },
        { "Crystal Cave", "The walls shimmer with crystalline formations" },
        { "Ancient Hall", "Grand pillars reach toward a vaulted ceiling" },
        { "Shadow Corridor", "Shadows dance on the stone walls" },
        { "Mystic Shrine", "Strange runes glow with ethereal light" },
        { "Forgotten Crypt", "The air is thick with the scent of decay" },
        { "Sacred Sanctum", "A holy place now abandoned and silent" },
        { "Hidden Vault", "Ancient treasures lie hidden here" },
        { "Cursed Throne Room", "An ominous throne dominates the chamber" },
        { "Abandoned Library", "Dusty tomes line the crumbling shelves" },
        { "Torture Chamber", "Rusty chains hang from the blood-stained walls" },
        { "Treasury", "Gold and jewels glitter in the dim light" },
        { "Ritual Circle", "Arcane symbols are carved into the floor" },
        { "War Room", "Old battle plans still hang on the walls" },
        { "Armory", "Weapons of ages past rest in their racks" },
        { "Dragon's Lair", "The heat is oppressive and the air smells of sulfur" }
    };

            var selectedRoom = roomTemplates.ElementAt(random.Next(roomTemplates.Count));
            var name = selectedRoom.Key;
            var description = selectedRoom.Value;

            var baseExp = dungeonLevel * 20;
            var baseGold = dungeonLevel * 10;

            var difficultyMultiplier = difficulty switch
            {
                DifficultyLevelEnum.EASY => 1.0,
                DifficultyLevelEnum.MEDIUM => 1.5,
                DifficultyLevelEnum.HARD => 2.0,
                DifficultyLevelEnum.EXTREME => 3.0,
                _ => 1.0
            };

            var experienceGained = (int)(baseExp * difficultyMultiplier);
            var goldGained = (int)(baseGold * difficultyMultiplier);

            var room = new Room
            {
                Name = name,
                Level = dungeonLevel,
                Description = description,
                DifficultyLevel = difficulty,
                ExperienceGained = experienceGained,
                GoldGained = goldGained,
                IsExplored = false,
                Monster = null
            };

            if (random.Next(100) < 80)
            {
                var monster = await _monsterService.GenerateMonsterForLevel(dungeonLevel);
                room.Monster = monster;
            }

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return room;
        }
    }
}