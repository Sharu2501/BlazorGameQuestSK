using BlazorGameAPI.Services;
using SharedModels.Model;

namespace BlazorGame.Tests.GameTest
{
    public class PlayerGameSessionTests : TestBase
    {
        [Fact]
        public async Task StartSession_CreatesActiveSession()
        {
            using var ctx = CreateInMemoryContext();

           var player = new Player { Username = "gamer", Email = "g@blazorgame.com", PasswordHash = "1234" };
            ctx.Players.Add(player);

            var room1 = new Room { Name = "R1", Description = "Chambre 1" };
            var room2 = new Room { Name = "R2", Description = "Chambre 2" };
            var dungeon = new Dungeon { Name = "D1", Description = "D", Rooms = new List<Room> { room1, room2 } };
            ctx.Dungeons.Add(dungeon);

            await ctx.SaveChangesAsync();

            var svc = new GameSessionService(ctx);

            var session = await svc.StartSession(player.Id, dungeon.IdDungeon);

            Assert.NotNull(session);
            Assert.Equal(player.Id, session.PlayerId);
            Assert.Equal(dungeon.IdDungeon, session.CurrentDungeonId);
            Assert.True(session.CurrentRoomId == room1.Id || session.CurrentRoomId == room1.Id);
            Assert.True(session.IsActive);

            var active = await svc.GetActiveSession(player.Id);
            Assert.NotNull(active);
            Assert.Equal(session.SessionId, active!.SessionId);
        }

        [Fact]
        public async Task Move_Save_And_EndSession_Works()
        {
            using var ctx = CreateInMemoryContext();

            var player = new Player { Username = "gamer2", Email = "g2@blazorgame.com", PasswordHash = "1234" };
            ctx.Players.Add(player);

            var room1 = new Room { Name = "RoomA", Description = "Chambre A" };
            var room2 = new Room { Name = "RoomB", Description = "Chambre B" };
            var dungeon = new Dungeon { Name = "Dn", Description = "desc", Rooms = new List<Room> { room1, room2 } };
            ctx.Dungeons.Add(dungeon);
            await ctx.SaveChangesAsync();

            var svc = new GameSessionService(ctx);
            var session = await svc.StartSession(player.Id, dungeon.IdDungeon);

            var moved = await svc.MoveToRoom(session.SessionId, room2.Id);
            Assert.True(moved);

            var fromDb = await svc.GetSessionById(session.SessionId);
            Assert.Equal(room2.Id, fromDb!.CurrentRoomId);

            var saved = await svc.SaveSessionAsync(session.SessionId, "{\"dummy\":1}", true);
            Assert.True(saved);

            var updated = await svc.GetSessionById(session.SessionId);
            Assert.Equal("{\"dummy\":1}", updated!.StateJson);
            Assert.True(updated.IsPaused);

            var ended = await svc.EndSession(session.SessionId);
            Assert.True(ended);
            Assert.False(await svc.IsSessionActive(session.SessionId));
        }
    }
}
