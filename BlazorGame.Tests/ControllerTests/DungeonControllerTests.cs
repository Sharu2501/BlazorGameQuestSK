using Microsoft.AspNetCore.Mvc;
using SharedModels.Model;
using BlazorGameAPI.Services;
using BlazorGameAPI.Controllers;

namespace BlazorGame.Tests.ControllersTests
{
    public class DungeonControllerTests : TestBase
    {
        [Fact]
        public async Task GetDungeons_ReturnsOk_WithListAndRooms()
        {
            using var ctx = CreateInMemoryContext();
            var d1 = new Dungeon { Name = "D1", Rooms = new List<Room> { new Room { Name = "R1" } } };
            var d2 = new Dungeon { Name = "D2", Rooms = new List<Room>() };
            ctx.Dungeons.AddRange(d1, d2);
            ctx.SaveChanges();  

            var monsterService = new MonsterService(ctx);
            var artifactService = new ArtifactService(ctx);
            var roomService = new RoomService(ctx, monsterService);
            var dungeonService = new DungeonService(ctx, roomService, artifactService);
            var controller = new DungeonController(ctx, dungeonService);
            var result = await controller.GetDungeons();

            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<List<Dungeon>>(ok.Value);
            Assert.Equal(2, list.Count);
            Assert.Contains(list, d => d.Name == "D1" && d.Rooms.Count == 1);
        }

        [Fact]
        public async Task GetDungeon_ReturnsNotFound_WhenMissing_And_Ok_WhenExists()
        {
            using var ctx = CreateInMemoryContext();
            var monsterService = new MonsterService(ctx);
            var artifactService = new ArtifactService(ctx);
            var roomService = new RoomService(ctx, monsterService);
            var dungeonService = new DungeonService(ctx, roomService, artifactService);
            var controller = new DungeonController(ctx, dungeonService);

            var nf = await controller.GetDungeon(99999);
            Assert.IsType<NotFoundResult>(nf);

            var d = new Dungeon { Name = "Single" };
            ctx.Dungeons.Add(d);
            ctx.SaveChanges();

            var okRes = await controller.GetDungeon(d.IdDungeon);
            var ok = Assert.IsType<OkObjectResult>(okRes);
            var returned = Assert.IsType<Dungeon>(ok.Value);
            Assert.Equal("Single", returned.Name);
        }

        [Fact]
        public async Task CreateDungeon_ReturnsCreated_And_Persists()
        {
            using var ctx = CreateInMemoryContext();
            var monsterService = new MonsterService(ctx);
            var artifactService = new ArtifactService(ctx);
            var roomService = new RoomService(ctx, monsterService);
            var dungeonService = new DungeonService(ctx, roomService, artifactService);
            var controller = new DungeonController(ctx, dungeonService);

            var dungeon = new Dungeon { Name = "NewDungeon", Rooms = new List<Room>() };
            var res = await controller.CreateDungeon(dungeon);

            var created = Assert.IsType<CreatedAtActionResult>(res);
            var returned = Assert.IsType<Dungeon>(created.Value);
            Assert.Equal("NewDungeon", returned.Name);

            var fromDb = await ctx.Dungeons.FindAsync(returned.IdDungeon);
            Assert.NotNull(fromDb);
            Assert.Equal("NewDungeon", fromDb.Name);
        }

        [Fact]
        public async Task DeleteDungeon_ReturnsNotFound_WhenMissing_And_NoContent_WhenDeleted()
        {
            using var ctx = CreateInMemoryContext();
            var monsterService = new MonsterService(ctx);
            var artifactService = new ArtifactService(ctx);
            var roomService = new RoomService(ctx, monsterService);
            var dungeonService = new DungeonService(ctx, roomService, artifactService);
            var controller = new DungeonController(ctx, dungeonService);

            var nf = await controller.DeleteDungeon(99999);
            Assert.IsType<NotFoundResult>(nf);

            var d = new Dungeon { Name = "ToDelete" };
            ctx.Dungeons.Add(d);
            ctx.SaveChanges();

            var del = await controller.DeleteDungeon(d.IdDungeon);
            Assert.IsType<NoContentResult>(del);

            var after = await ctx.Dungeons.FindAsync(d.IdDungeon);
            Assert.Null(after);
        }
        [Fact]
        public async Task GenerateDungeon_ReturnsOk_WithGeneratedDungeon()
        {
            using var ctx = CreateInMemoryContext();
            var monsterService = new MonsterService(ctx);
            var artifactService = new ArtifactService(ctx);
            var roomService = new RoomService(ctx, monsterService);
            var dungeonService = new DungeonService(ctx, roomService, artifactService);
            var controller = new DungeonController(ctx, dungeonService);

            var req = new GenerateDungeonRequest { NumberOfRooms = 3, Level = 1 };
            var res = await controller.GenerateDungeon(req);
            var ok = Assert.IsType<OkObjectResult>(res);
            var dungeon = Assert.IsType<Dungeon>(ok.Value);
            Assert.NotNull(dungeon);
            Assert.Equal(3, dungeon.Rooms.Count);
        }
    }
}