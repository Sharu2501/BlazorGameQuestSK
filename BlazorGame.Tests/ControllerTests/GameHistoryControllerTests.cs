using BlazorGameAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedModels.Model;
using BlazorGameAPI.Controllers;

namespace BlazorGame.Tests.ControllersTests
{
    public class GameHistoryControllerTests : TestBase
    {
        private GameHistoryController CreateController(ApplicationDbContext ctx)
        {
            return new GameHistoryController(ctx);
        }

        [Fact]
        public async Task GetGameHistories_ReturnsOk_WithList()
        {
            using var ctx = CreateInMemoryContext();
            var player = new Player { Username = "p1" };
            var dungeon = new Dungeon { Name = "Donjon 1" };
            ctx.Players.Add(player);
            ctx.Dungeons.Add(dungeon);

            var history = new GameHistory
            {
                Player = player,
                CompletedDungeons = new List<Dungeon> { dungeon }
            };
            ctx.GameHistories.Add(history);
            ctx.SaveChanges();

            var controller = CreateController(ctx);

            var result = await controller.GetGameHistories();

            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<List<GameHistory>>(ok.Value);
            Assert.Single(list);
            Assert.Equal("p1", list[0].Player.Username);
        }

        [Fact]
        public async Task GetGameHistory_ReturnsNotFound_WhenMissing()
        {
            using var ctx = CreateInMemoryContext();
            var controller = CreateController(ctx);

            var result = await controller.GetGameHistory(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetGameHistory_ReturnsOk_WhenExists()
        {
            using var ctx = CreateInMemoryContext();
            var player = new Player { Username = "p1" };
            ctx.Players.Add(player);
            var history = new GameHistory { Player = player };
            ctx.GameHistories.Add(history);
            ctx.SaveChanges();

            var controller = CreateController(ctx);

            var result = await controller.GetGameHistory(history.Id);

            var ok = Assert.IsType<OkObjectResult>(result);
            var gh = Assert.IsType<GameHistory>(ok.Value);
            Assert.Equal(history.Id, gh.Id);
            Assert.Equal("p1", gh.Player.Username);
        }

        [Fact]
        public async Task GetPlayerGameHistories_ReturnsOnlyPlayerHistories()
        {
            using var ctx = CreateInMemoryContext();
            var p1 = new Player { Username = "p1" };
            var p2 = new Player { Username = "p2" };
            ctx.Players.AddRange(p1, p2);

            ctx.GameHistories.Add(new GameHistory { Player = p1 });
            ctx.GameHistories.Add(new GameHistory { Player = p1 });
            ctx.GameHistories.Add(new GameHistory { Player = p2 });
            ctx.SaveChanges();

            var controller = CreateController(ctx);

            var result = await controller.GetPlayerGameHistories(p1.Id);

            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<List<GameHistory>>(ok.Value);
            Assert.Equal(2, list.Count);
            Assert.All(list, gh => Assert.Equal(p1.Id, gh.Player.Id));
        }

        [Fact]
        public async Task CreateGameHistory_Returns_NotFound_When_PlayerMissing()
        {
            using var ctx = CreateInMemoryContext();
            var controller = CreateController(ctx);

            var req = new CreateGameHistoryRequest
            {
                PlayerId = 999,
                DungeonIds = new List<int> { 1, 2 }
            };

            var result = await controller.CreateGameHistory(req);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateGameHistory_Creates_History_Without_Dungeons()
        {
            using var ctx = CreateInMemoryContext();
            var player = new Player { Username = "p1" };
            ctx.Players.Add(player);
            ctx.SaveChanges();

            var controller = CreateController(ctx);

            var req = new CreateGameHistoryRequest
            {
                PlayerId = player.Id,
                DungeonIds = null
            };

            var result = await controller.CreateGameHistory(req);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            var history = Assert.IsType<GameHistory>(created.Value);
            Assert.Equal(player.Id, history.Player.Id);
            Assert.Empty(history.CompletedDungeons);

            var fromDb = await ctx.GameHistories
                .Include(h => h.Player)
                .Include(h => h.CompletedDungeons)
                .FirstOrDefaultAsync(h => h.Id == history.Id);

            Assert.NotNull(fromDb);
            Assert.Equal(player.Id, fromDb!.Player.Id);
            Assert.Empty(fromDb.CompletedDungeons);
        }

        [Fact]
        public async Task CreateGameHistory_Creates_History_With_Selected_Dungeons()
        {
            using var ctx = CreateInMemoryContext();
            var player = new Player { Username = "p1" };
            var d1 = new Dungeon { Name = "D1" };
            var d2 = new Dungeon { Name = "D2" };
            ctx.Players.Add(player);
            ctx.Dungeons.AddRange(d1, d2);
            ctx.SaveChanges();

            var controller = CreateController(ctx);

            var req = new CreateGameHistoryRequest
            {
                PlayerId = player.Id,
                DungeonIds = new List<int> { d1.IdDungeon, d2.IdDungeon }
            };

            var result = await controller.CreateGameHistory(req);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            var history = Assert.IsType<GameHistory>(created.Value);
            Assert.Equal(2, history.CompletedDungeons.Count);

            var fromDb = await ctx.GameHistories
                .Include(h => h.CompletedDungeons)
                .FirstOrDefaultAsync(h => h.Id == history.Id);

            Assert.NotNull(fromDb);
            Assert.Equal(2, fromDb!.CompletedDungeons.Count);
        }
    }
}