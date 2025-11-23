using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using BlazorGameAPI.Data;
using SharedModels.Model;

namespace BlazorGame.Tests.ControllersTests
{
    public class PlayerControllerTests : TestBase
    {
        [Fact]
        public async Task GetPlayers_ReturnsOk_WithList()
        {
            using var ctx = CreateInMemoryContext();
            ctx.Players.Add(new Player { Username = "p1" });
            ctx.Players.Add(new Player { Username = "p2" });
            ctx.SaveChanges();

            var service = new BlazorGameAPI.Services.PlayerService(ctx);
            var controller = new PlayerController(ctx,service);
            var result = await controller.GetPlayers();

            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<System.Collections.Generic.List<Player>>(ok.Value);
            Assert.Equal(2, list.Count);
        }

        [Fact]
        public async Task CreatePlayer_ReturnsCreatedAndPersists()
        {
            using var ctx = CreateInMemoryContext();
            var service = new BlazorGameAPI.Services.PlayerService(ctx);
            var controller = new PlayerController(ctx, service);

            var player = new Player { Username = "new", Email = "a@b.c" };
            var res = await controller.CreatePlayer(player);

            var created = Assert.IsType<CreatedAtActionResult>(res);
            var returned = Assert.IsType<Player>(created.Value);
            Assert.Equal("new", returned.Username);

            var fromDb = await ctx.Players.FindAsync(returned.Id);
            Assert.NotNull(fromDb);
            Assert.Equal("new", fromDb.Username);
        }

        [Fact]
        public async Task GetPlayerStats_ReturnsNotFound_WhenMissing_And_Ok_WhenExists()
        {
            using var ctx = CreateInMemoryContext();
             var service = new BlazorGameAPI.Services.PlayerService(ctx);
            var controller = new PlayerController(ctx, service);

            var notFound = await controller.GetPlayerStats(12345);
            Assert.IsType<NotFoundResult>(notFound.Result);

            var p = new Player { Username = "stat", Level = 2, ExperiencePoints = 10, LevelCap = 100, Gold = 5, Health = 20, MaxHealth = 30, Attack = 4, Defense = 1 };
            ctx.Players.Add(p);
            ctx.SaveChanges();

            var okResult = await controller.GetPlayerStats(p.Id);
            var ok = Assert.IsType<OkObjectResult>(okResult.Result);
            var dto = Assert.IsType<SharedModels.Model.DTOs.PlayerStatsDto>(ok.Value);
            Assert.Equal(p.Username, dto.Username);
            Assert.Equal(p.Level, dto.Level);
        }

        [Fact]
        public async Task DeletePlayer_ReturnsNotFound_WhenMissing_And_NoContent_WhenDeleted()
        {
            using var ctx = CreateInMemoryContext(); 
            var service = new BlazorGameAPI.Services.PlayerService(ctx);
            var controller = new PlayerController(ctx, service);

            var nf = await controller.DeletePlayer(99999);
            Assert.IsType<NotFoundResult>(nf);

            var p = new Player { Username = "delme" };
            ctx.Players.Add(p);
            ctx.SaveChanges();

            var del = await controller.DeletePlayer(p.Id);
            Assert.IsType<NoContentResult>(del);

            var after = await ctx.Players.FindAsync(p.Id);
            Assert.Null(after);
        }
    }
}