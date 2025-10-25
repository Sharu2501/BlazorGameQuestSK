using BlazorGameAPI.Data;
using BlazorGameAPI.Services;
using Microsoft.EntityFrameworkCore;
using SharedModels.Enum;
using SharedModels.Model;
using Xunit;

namespace BlazorGame.Tests
{
    public class RoomTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task CreateRoom_ShouldAddRoomToDatabase()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var monsterService = new MonsterService(context); // Mock ou service réel
            var roomService = new RoomService(context, monsterService);

            // Act
            var room = await roomService.CreateRoom(
                "Test Room",
                1,
                "A room for testing",
                DifficultyLevelEnum.MEDIUM,
                50,
                20
            );

            // Assert
            Assert.NotNull(room);
            Assert.Equal("Test Room", room.Name);
            Assert.Equal(1, room.Level);
            Assert.Equal("A room for testing", room.Description);
            Assert.Equal(DifficultyLevelEnum.MEDIUM, room.DifficultyLevel);
            Assert.Equal(50, room.ExperienceGained);
            Assert.Equal(20, room.GoldGained);

            var savedRoom = context.Rooms.FirstOrDefault(r => r.Id == room.Id);
            Assert.NotNull(savedRoom);
        }

        [Fact]
        public async Task MarkRoomAsExplored_ShouldSetIsExploredToTrue()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var monsterService = new MonsterService(context); // Mock ou service réel
            var roomService = new RoomService(context, monsterService);

            var room = await roomService.CreateRoom(
                "Test Room",
                1,
                "A room for testing",
                DifficultyLevelEnum.MEDIUM,
                50,
                20
            );

            // Act
            var result = await roomService.MarkRoomAsExplored(room.Id);

            // Assert
            Assert.True(result);
            var updatedRoom = context.Rooms.FirstOrDefault(r => r.Id == room.Id);
            Assert.NotNull(updatedRoom);
            Assert.True(updatedRoom.IsExplored);
        }

        [Fact]
        public async Task AssignMonsterToRoom_ShouldAssignMonster()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var monsterService = new MonsterService(context); // Mock ou service réel
            var roomService = new RoomService(context, monsterService);

            var room = await roomService.CreateRoom(
                "Test Room",
                1,
                "A room for testing",
                DifficultyLevelEnum.MEDIUM,
                50,
                20
            );

            var monster = new Monster
            {
                IdMonster = 1,
                Name = "Test Monster",
                Health = 100,
                Attack = 10,
                Defense = 5
            };

            context.Monsters.Add(monster);
            await context.SaveChangesAsync();

            // Act
            var result = await roomService.AssignMonsterToRoom(room.Id, monster.IdMonster);

            // Assert
            Assert.True(result);
            var updatedRoom = context.Rooms.Include(r => r.Monster).FirstOrDefault(r => r.Id == room.Id);
            Assert.NotNull(updatedRoom);
            Assert.NotNull(updatedRoom.Monster);
            Assert.Equal("Test Monster", updatedRoom.Monster.Name);
        }
    }
}