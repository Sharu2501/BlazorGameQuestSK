using Microsoft.EntityFrameworkCore;
using SharedModels.Model;
using BlazorGameAPI.Data;
using BlazorGameAPI.Services;
using Xunit;

namespace BlazorGame.Tests.ModelsTests
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
    }
}