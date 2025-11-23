using BlazorGameAPI.Services;
using SharedModels.Model;

namespace BlazorGame.Tests.ServicesTests
{
    public class DungeonServiceTests : TestBase
    {
        [Fact]
        public async Task CreateDungeon_And_GetDungeonById_Works()
        {
            using var ctx = CreateInMemoryContext();
            var monsterService = new MonsterService(ctx);
            var roomService = new RoomService(ctx, monsterService);
            var artifactService = new ArtifactService(ctx);
            var dungeonService = new DungeonService(ctx, roomService, artifactService);

            var d = await dungeonService.CreateDungeon("Donjon", "desc");
            Assert.NotNull(d);

            var fetched = await dungeonService.GetDungeonById(d.IdDungeon);
            Assert.NotNull(fetched);
            Assert.Equal("Donjon", fetched.Name);
        }

        [Fact]
        public async Task AddRoomToDungeon_And_GetAllDungeons_ExplorationFilters()
        {
            using var ctx = CreateInMemoryContext();
            var monsterService = new MonsterService(ctx);
            var roomService = new RoomService(ctx, monsterService);
            var artifactService = new ArtifactService(ctx);
            var dungeonService = new DungeonService(ctx, roomService, artifactService);

            var d = await dungeonService.CreateDungeon("D1", "x");
            var room = new Room { Name = "r", Level = 1, Description = "d" };
            ctx.Rooms.Add(room);
            ctx.SaveChanges();

            var added = await dungeonService.AddRoomToDungeon(d.IdDungeon, room);
            Assert.True(added);

            var all = await dungeonService.GetAllDungeons();
            Assert.Contains(all, x => x.IdDungeon == d.IdDungeon);

            var notExplored = await dungeonService.GetDungeonsByExplorationStatus(false);
            Assert.Contains(notExplored, x => x.IdDungeon == d.IdDungeon);
        }

        [Fact]
        public async Task AssignArtifactToDungeon_MarkAsExplored_IsCompleted()
        {
            using var ctx = CreateInMemoryContext();
            var monsterService = new MonsterService(ctx);
            var roomService = new RoomService(ctx, monsterService);
            var artifactService = new ArtifactService(ctx);
            var dungeonService = new DungeonService(ctx, roomService, artifactService);

            var d = await dungeonService.CreateDungeon("D2", "x");
            var artifact = new Artifact { Name = "A", Description = "desc", Rarity = SharedModels.Enum.RarityEnum.COMMON };
            ctx.Artifacts.Add(artifact);
            ctx.SaveChanges();

            var ok = await dungeonService.AssignArtifactToDungeon(d.IdDungeon, artifact.Id);
            Assert.True(ok);

            var mark = await dungeonService.MarkDungeonAsExplored(d.IdDungeon);
            Assert.True(mark);

            var completed = await dungeonService.IsDungeonCompleted(d.IdDungeon);
            Assert.True(completed);
        }

        [Fact]
        public async Task GetDungeonProgress_ComputesPercentage()
        {
            using var ctx = CreateInMemoryContext();
            var monsterService = new MonsterService(ctx);
            var roomService = new RoomService(ctx, monsterService);
            var artifactService = new ArtifactService(ctx);
            var dungeonService = new DungeonService(ctx, roomService, artifactService);

            var d = new Dungeon { Name = "Prog", Description = "x", Rooms = new System.Collections.Generic.List<Room>() };
            ctx.Dungeons.Add(d);
            ctx.SaveChanges();

            var r1 = new Room { Name = "r1", Level = 1, IsExplored = true };
            var r2 = new Room { Name = "r2", Level = 1, IsExplored = false };
            var r3 = new Room { Name = "r3", Level = 1, IsExplored = true };

            ctx.Rooms.AddRange(r1, r2, r3);
            ctx.SaveChanges();

            d.Rooms.Add(r1);
            d.Rooms.Add(r2);
            d.Rooms.Add(r3);
            ctx.SaveChanges();

            var progress = await dungeonService.GetDungeonProgress(d.IdDungeon);
            Assert.InRange(progress, 66, 67);
        }

        [Fact]
        public async Task UpdateDungeon_And_DeleteDungeon_Work()
        {
            using var ctx = CreateInMemoryContext();
            var monsterService = new MonsterService(ctx);
            var roomService = new RoomService(ctx, monsterService);
            var artifactService = new ArtifactService(ctx);
            var dungeonService = new DungeonService(ctx, roomService, artifactService);

            var d = await dungeonService.CreateDungeon("ToUpd", "x");
            d.Name = "Updated";
            var ok = await dungeonService.UpdateDungeon(d);
            Assert.True(ok);

            var fromDb = await ctx.Dungeons.FindAsync(d.IdDungeon);
            Assert.Equal("Updated", fromDb.Name);

            var del = await dungeonService.DeleteDungeon(d.IdDungeon);
            Assert.True(del);

            var after = await ctx.Dungeons.FindAsync(d.IdDungeon);
            Assert.Null(after);
        }
    }
}