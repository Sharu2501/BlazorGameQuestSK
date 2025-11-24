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
        /// <summary>
        /// Récupère toutes les salles d'un donjon donné.
        /// </summary>
        /// <param name="dungeonId"></param>
        /// <returns></returns>
        public Task<List<Room>> GetRoomsByDungeonId(int dungeonId)
        {
            List<Room> rooms = _context.Rooms
                .Where(r => r.Id == dungeonId)
                .ToList();
            return Task.FromResult(rooms);
        }
        /// <summary>
        /// Assigne un monstre à une salle.
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="monsterId"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Marque une salle comme explorée.
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Vérifie si une salle a été explorée.
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        public Task<bool> IsRoomExplored(int roomId)
        {
            var room = _context.Rooms.Find(roomId);
            return Task.FromResult(room?.IsExplored ?? false);
        }
        /// <summary>
        /// Met à jour les informations d'une salle.
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Supprime une salle par son ID.
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
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

            Dictionary<string, string> roomTemplates = new()
            {
                { "Chambre Sombre", "Un passage obscur empli de mystères anciens" },
                { "Grotte de Cristal", "Les parois scintillent de formations cristallines" },
                { "Salle Ancienne", "De grands piliers soutiennent un plafond voûté" },
                { "Couloir des Ombres", "Les ombres dansent sur les murs de pierre" },
                { "Sanctuaire Mystique", "D’étranges runes brillent d’une lueur éthérée" },
                { "Crypte Oubliée", "L’air est lourd de l’odeur de la décomposition" },
                { "Sanctum Sacré", "Un lieu sacré désormais abandonné et silencieux" },
                { "Coffre Caché", "De anciens trésors sont dissimulés ici" },
                { "Salle du Trône Maudit", "Un trône inquiétant domine la pièce" },
                { "Bibliothèque Abandonnée", "Des tomes poussiéreux garnissent les étagères en ruine" },
                { "Chambre de Torture", "Des chaînes rouillées pendent des murs tachés de sang" },
                { "Trésorerie", "L’or et les joyaux brillent faiblement dans la pénombre" },
                { "Cercle Rituel", "Des symboles arcaniques sont gravés dans le sol" },
                { "Salle de Guerre", "De vieux plans de bataille couvrent encore les murs" },
                { "Armurerie", "Des armes d’un autre âge reposent dans leurs râteliers" },
                { "Tanière du Dragon", "La chaleur est écrasante et l’air sent le soufre" }
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
                var monster = await _monsterService.GenerateMonsterForLevel(dungeonLevel, difficulty);
                room.Monster = monster;
            }

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return room;
        }
    }
}