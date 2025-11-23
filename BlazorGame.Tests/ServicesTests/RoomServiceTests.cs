using Microsoft.EntityFrameworkCore;
using BlazorGameAPI.Services;
using SharedModels.Enum;
using SharedModels.Model;

namespace BlazorGame.Tests.ServicesTests
{
    public class RoomServiceTests : TestBase
    {
        [Fact]
        public async Task CreateRoom_And_GetRoomById_Works()
        {
            using var ctx = CreateInMemoryContext();
            var monsterService = new MonsterService(ctx);
            var roomService = new RoomService(ctx, monsterService);

            var created = await roomService.CreateRoom("SalleTest", 1, "desc", DifficultyLevelEnum.EASY, 5, 2);
            Assert.NotNull(created);
            Assert.Equal("SalleTest", created.Name);

            var fetched = await roomService.GetRoomById(created.Id);
            Assert.NotNull(fetched);
            Assert.Equal(created.Id, fetched.Id);
        }

        [Fact]
        public async Task GetRoomsByDungeonId_ReturnsRoom_WhenIdsMatch()
        {
            using var ctx = CreateInMemoryContext();
            var monsterService = new MonsterService(ctx);
            var roomService = new RoomService(ctx, monsterService);

            var room = await roomService.CreateRoom("R", 1, "d", DifficultyLevelEnum.EASY, 0, 0);
            var list = await roomService.GetRoomsByDungeonId(room.Id);
            Assert.Single(list);
            Assert.Equal(room.Id, list.First().Id);
        }

        [Fact]
        public async Task AssignMonsterToRoom_MarksMonster()
        {
            using var ctx = CreateInMemoryContext();
            var monsterService = new MonsterService(ctx);
            var roomService = new RoomService(ctx, monsterService);

            var room = await roomService.CreateRoom("R2", 1, "d", DifficultyLevelEnum.EASY, 0, 0);
            var monster = new Monster { Name = "Gob", Level = 1, Type = MonsterTypeEnum.GOBLIN };
            ctx.Monsters.Add(monster);
            ctx.SaveChanges();

            var ok = await roomService.AssignMonsterToRoom(room.Id, monster.IdMonster);
            Assert.True(ok);

            var reloaded = await ctx.Rooms.Include(r => r.Monster).FirstOrDefaultAsync(r => r.Id == room.Id);
            Assert.NotNull(reloaded.Monster);
            Assert.Equal(monster.IdMonster, reloaded.Monster.IdMonster);
        }

        [Fact]
        public async Task MarkRoomAsExplored_And_IsRoomExplored_Work()
        {
            using var ctx = CreateInMemoryContext();
            var monsterService = new MonsterService(ctx);
            var roomService = new RoomService(ctx, monsterService);

            var room = await roomService.CreateRoom("R3", 1, "d", DifficultyLevelEnum.EASY, 0, 0);
            var ok = await roomService.MarkRoomAsExplored(room.Id);
            Assert.True(ok);

            var isExplored = await roomService.IsRoomExplored(room.Id);
            Assert.True(isExplored);
        }

        [Fact]
        public async Task UpdateRoom_And_DeleteRoom_Work()
        {
            using var ctx = CreateInMemoryContext();
            var monsterService = new MonsterService(ctx);
            var roomService = new RoomService(ctx, monsterService);

            var room = await roomService.CreateRoom("Old", 1, "d", DifficultyLevelEnum.EASY, 0, 0);
            room.Name = "NewName";
            room.IsExplored = true;

            var updated = await roomService.UpdateRoom(room);
            Assert.True(updated);

            var fromDb = await ctx.Rooms.FindAsync(room.Id);
            Assert.Equal("NewName", fromDb.Name);
            Assert.True(fromDb.IsExplored);

            var deleted = await roomService.DeleteRoom(room.Id);
            Assert.True(deleted);

            var after = await ctx.Rooms.FindAsync(room.Id);
            Assert.Null(after);
        }
    }
}