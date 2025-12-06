using BlazorGameAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedModels.Model;
using BlazorGameAPI.Controllers;
using BlazorGameAPI.Services;

namespace BlazorGame.Tests.ControllersTests
{
    public class GameSessionControllerTests : TestBase
    {
        private GameSessionController CreateController(ApplicationDbContext ctx)
        {
            GameSessionService service = new GameSessionService(ctx);
            return new GameSessionController(ctx, service);
        }

        [Fact]
        public async Task StartSession_Creates_Session_And_Returns_CreatedAtAction()
        {
            using var ctx = CreateInMemoryContext();
            var controller = CreateController(ctx);

            var session = new GameSession
            {
                SessionId = 0,
                IsActive = true
            };

            var result = await controller.StartSession(session);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            var returned = Assert.IsType<GameSession>(created.Value);
            Assert.NotEqual(0, returned.SessionId);
            Assert.True(returned.IsActive);

            var fromDb = await ctx.GameSessions.FirstOrDefaultAsync(gs => gs.SessionId == returned.SessionId);
            Assert.NotNull(fromDb);
            Assert.True(fromDb!.IsActive);
        }

        [Fact]
        public async Task GetSession_Returns_NotFound_When_Missing()
        {
            using var ctx = CreateInMemoryContext();
            var controller = CreateController(ctx);

            var result = await controller.GetSession(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetSession_Returns_Ok_When_Exists()
        {
            using var ctx = CreateInMemoryContext();

            var session = new GameSession
            {
                IsActive = true
            };
            ctx.GameSessions.Add(session);
            ctx.SaveChanges();

            var controller = CreateController(ctx);

            var result = await controller.GetSession(session.SessionId);

            var ok = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<GameSession>(ok.Value);
            Assert.Equal(session.SessionId, returned.SessionId);
            Assert.True(returned.IsActive);
        }

        [Fact]
        public async Task EndSession_Returns_NotFound_When_Missing()
        {
            using var ctx = CreateInMemoryContext();
            var controller = CreateController(ctx);

            var result = await controller.EndSession(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task EndSession_Sets_IsActive_False_And_Returns_NoContent()
        {
            using var ctx = CreateInMemoryContext();

            var session = new GameSession
            {
                IsActive = true
            };
            ctx.GameSessions.Add(session);
            ctx.SaveChanges();

            var controller = CreateController(ctx);

            var result = await controller.EndSession(session.SessionId);

            Assert.IsType<NoContentResult>(result);

            var fromDb = await ctx.GameSessions.FirstOrDefaultAsync(gs => gs.SessionId == session.SessionId);
            Assert.NotNull(fromDb);
            Assert.False(fromDb!.IsActive);
        }
    }
}