using Microsoft.EntityFrameworkCore;
using SharedModels.Model;
using BlazorGameAPI.Data;
using BlazorGameAPI.Services;
using Xunit;

namespace BlazorGame.Tests
{
    public class DungeonTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task CreateDungeon_ShouldAddDungeonToDatabase()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var monsterService = new MonsterService(context);
            var roomService = new RoomService(context, monsterService);
            var artifactService = new ArtifactService(context);
            var dungeonService = new DungeonService(context, roomService, artifactService);

            // Act
            var dungeon = await dungeonService.CreateDungeon("Test Dungeon", "A dungeon for testing");

            // Assert
            Assert.NotNull(dungeon);
            Assert.Equal("Test Dungeon", dungeon.Name);
            Assert.Equal("A dungeon for testing", dungeon.Description);
            Assert.False(dungeon.IsExplored);

            var savedDungeon = context.Dungeons.FirstOrDefault(d => d.IdDungeon == dungeon.IdDungeon);
            Assert.NotNull(savedDungeon);
        }

        [Fact]
        public async Task MarkDungeonAsExplored_ShouldSetIsExploredToTrue()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var monsterService = new MonsterService(context);
            var roomService = new RoomService(context, monsterService);
            var artifactService = new ArtifactService(context);
            var dungeonService = new DungeonService(context, roomService, artifactService);

            var dungeon = await dungeonService.CreateDungeon("Test Dungeon", "A dungeon for testing");

            // Act
            var result = await dungeonService.MarkDungeonAsExplored(dungeon.IdDungeon);

            // Assert
            Assert.True(result);
            var updatedDungeon = context.Dungeons.FirstOrDefault(d => d.IdDungeon == dungeon.IdDungeon);
            Assert.NotNull(updatedDungeon);
            Assert.True(updatedDungeon.IsExplored);
        }

        [Fact]
        public async Task GetDungeonProgress_ShouldReturnCorrectProgress()
        {
            var context = GetInMemoryDbContext();
            var monsterService = new MonsterService(context);
            var roomService = new RoomService(context, monsterService);
            var artifactService = new ArtifactService(context);
            var dungeonService = new DungeonService(context, roomService, artifactService);

            var dungeon = await dungeonService.CreateDungeon("Test Dungeon", "A dungeon for testing");

            var rooms = new List<Room>
            {
                new Room { Name = "Room 1", IsExplored = true },
                new Room { Name = "Room 2", IsExplored = false },
                new Room { Name = "Room 3", IsExplored = true }
            };

            foreach (var room in rooms)
            {
                context.Rooms.Add(room);
                context.Entry(room).Property("DungeonId").CurrentValue = dungeon.IdDungeon;
            }

            context.SaveChanges();

            var progress = await dungeonService.GetDungeonProgress(dungeon.IdDungeon);

            Assert.Equal(2 * 100 / 3, progress);
        }
    }
}