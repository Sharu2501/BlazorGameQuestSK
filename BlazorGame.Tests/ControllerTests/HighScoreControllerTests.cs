using BlazorGameAPI.Data;
using BlazorGameAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedModels.Model;
using BlazorGameAPI.Controllers;

namespace BlazorGame.Tests.ControllersTests
{
    public class HighScoreControllerTests : TestBase
    {
        private HighScoreController CreateController(ApplicationDbContext ctx)
        {
            var service = new HighScoreService(ctx);
            return new HighScoreController(ctx, service);
        }

        [Fact]
        public async Task GetHighScores_ReturnsOk_WithList()
        {
            using var ctx = CreateInMemoryContext();

            var p1 = new Player { Username = "p1" };
            var p2 = new Player { Username = "p2" };
            ctx.Players.AddRange(p1, p2);
            ctx.SaveChanges();

            p1.HighScore.Score = 100;
            p2.HighScore.Score = 200;
            ctx.SaveChanges();

            var controller = CreateController(ctx);

            var result = await controller.GetHighScores();
            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<List<HighScore>>(ok.Value);

            Assert.Equal(2, list.Count);
            Assert.Contains(list, hs => hs.Score == 100);
            Assert.Contains(list, hs => hs.Score == 200);
        }

        [Fact]
        public async Task GetHighScore_ReturnsNotFound_WhenMissing()
        {
            using var ctx = CreateInMemoryContext();
            var controller = CreateController(ctx);

            var result = await controller.GetHighScore(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetHighScore_ReturnsOk_WhenExists()
        {
            using var ctx = CreateInMemoryContext();

            var player = new Player { Username = "p1" };
            ctx.Players.Add(player);

            var hs = new HighScore {Score = 150 };
            player.HighScore = hs;
            ctx.HighScores.Add(hs);
            ctx.SaveChanges();

            var controller = CreateController(ctx);

            var result = await controller.GetHighScore(hs.Id);

            var ok = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<HighScore>(ok.Value);
            Assert.Equal(hs.Id, returned.Id);
            Assert.Equal(150, returned.Score);
        }

        [Fact]
        public async Task UpdateHighScore_CallsService_AndReturnsOk()
        {
            using var ctx = CreateInMemoryContext();

            var player = new Player { Username = "p1" };
            ctx.Players.Add(player);
            ctx.SaveChanges();

            var service = new HighScoreService(ctx);
            var controller = new HighScoreController(ctx, service);

            var req = new UpdateHighScoreRequest
            {
                PlayerId = player.Id,
                Score = 300
            };

            var result = await controller.UpdateHighScore(req);

            var ok = Assert.IsType<OkObjectResult>(result);
            var success = Assert.IsType<bool>(ok.Value);
            Assert.True(success);

            var fromDbPlayer = await ctx.Players
                .Include(p => p.HighScore)
                .FirstOrDefaultAsync(p => p.Id == player.Id);

            Assert.NotNull(fromDbPlayer);
            Assert.NotNull(fromDbPlayer!.HighScore);
            Assert.Equal(300, fromDbPlayer.HighScore.Score);

        }
    }
}