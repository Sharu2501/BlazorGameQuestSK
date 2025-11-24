using Microsoft.AspNetCore.Mvc;
using BlazorGameAPI.Controllers;
using BlazorGameAPI.Services;
using SharedModels.Model;
using SharedModels.Enum;

namespace BlazorGame.Tests.ControllersTests
{
    public class RoomControllerTests : TestBase
    {
        [Fact]
        public async Task GetRooms_ReturnsOk_WithList()
        {
            using var ctx = CreateInMemoryContext();
            ctx.Rooms.Add(new Room { Name = "R1" });
            ctx.Rooms.Add(new Room { Name = "R2" });
            ctx.SaveChanges();

            var monsterService = new MonsterService(ctx);
            var roomService = new RoomService(ctx, monsterService);
            var controller = new RoomController(ctx, roomService);

            var result = await controller.GetRooms();
            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<System.Collections.Generic.List<Room>>(ok.Value);
            Assert.Equal(2, list.Count);
        }

        [Fact]
        public async Task GetRoom_ReturnsNotFound_WhenMissing_And_Ok_WhenExists()
        {
            using var ctx = CreateInMemoryContext();
            var monsterService = new MonsterService(ctx);
            var roomService = new RoomService(ctx, monsterService);
            var controller = new RoomController(ctx, roomService);

            var nf = await controller.GetRoom(99999);
            Assert.IsType<NotFoundResult>(nf);

            var r = new Room { Name = "Single" };
            ctx.Rooms.Add(r);
            ctx.SaveChanges();

            var okRes = await controller.GetRoom(r.Id);
            var ok = Assert.IsType<OkObjectResult>(okRes);
            var returned = Assert.IsType<Room>(ok.Value);
            Assert.Equal("Single", returned.Name);
        }

        [Fact]
        public async Task CreateRoom_ReturnsCreatedAndPersists()
        {
            using var ctx = CreateInMemoryContext();
            var monsterService = new MonsterService(ctx);
            var roomService = new RoomService(ctx, monsterService);
            var controller = new RoomController(ctx, roomService);

            var room = new Room { Name = "NewRoom" };
            var res = await controller.CreateRoom(room);

            var created = Assert.IsType<CreatedAtActionResult>(res);
            var returned = Assert.IsType<Room>(created.Value);
            Assert.Equal("NewRoom", returned.Name);

            var fromDb = await ctx.Rooms.FindAsync(returned.Id);
            Assert.NotNull(fromDb);
            Assert.Equal("NewRoom", fromDb.Name);
        }

        [Fact]
        public async Task DeleteRoom_ReturnsNotFound_WhenMissing_And_NoContent_WhenDeleted()
        {
            using var ctx = CreateInMemoryContext();
            var monsterService = new MonsterService(ctx);
            var roomService = new RoomService(ctx, monsterService);
            var controller = new RoomController(ctx, roomService);

            var nf = await controller.DeleteRoom(99999);
            Assert.IsType<NotFoundResult>(nf);

            var r = new Room { Name = "ToDelete" };
            ctx.Rooms.Add(r);
            ctx.SaveChanges();

            var del = await controller.DeleteRoom(r.Id);
            Assert.IsType<NoContentResult>(del);

            var after = await ctx.Rooms.FindAsync(r.Id);
            Assert.Null(after);
        }

        [Fact]
        public async Task GenerateRoom_UsesService_And_ReturnsOkRoom()
        {
            using var ctx = CreateInMemoryContext();
            var monsterService = new MonsterService(ctx);
            var roomService = new RoomService(ctx, monsterService);
            var controller = new RoomController(ctx, roomService);

            var actionResult = await controller.GenerateRoom(1, (int)DifficultyLevelEnum.EASY);
            var ok = Assert.IsType<OkObjectResult>(actionResult.Result);
            var generated = Assert.IsType<Room>(ok.Value);
            Assert.NotNull(generated);
        }
    }
}