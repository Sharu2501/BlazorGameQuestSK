using BlazorGameAPI.Services;
using SharedModels.Model;
using SharedModels.Enum;

namespace BlazorGame.Tests.ServicesTests
{
    public class PlayerServiceTests : TestBase
    {
        [Fact]
        public async Task CreatePlayer_And_GetPlayerById_Works()
        {
            using var ctx = CreateInMemoryContext();
            var svc = new PlayerService(ctx);

            var created = await svc.CreatePlayer("testuser", "a@b.c", "hash");
            Assert.NotNull(created);
            Assert.Equal("testuser", created.Username);
            Assert.True(created.Id > 0);

            var fetched = await svc.GetPlayerById(created.Id);
            Assert.NotNull(fetched);
            Assert.Equal("testuser", fetched.Username);
        }

        [Fact]
        public async Task AddExperience_LevelUp_UpdatesStats()
        {
            using var ctx = CreateInMemoryContext();
            var svc = new PlayerService(ctx);

            var p = await svc.CreatePlayer("xpuser", "e@mail", "h");
            await svc.AddExperience(p.Id, 250);

            var after = await svc.GetPlayerById(p.Id);
            Assert.True(after.Level >= 2);
            Assert.True(after.MaxHealth >= 110);
            Assert.True(after.Attack >= 12);
        }

        [Fact]
        public async Task ChangeAction_And_ResetAction_Works()
        {
            using var ctx = CreateInMemoryContext();
            var svc = new PlayerService(ctx);

            var p = await svc.CreatePlayer("act", "a", "h");
            await svc.ChangeAction(p.Id, PlayerActionEnum.FIGHT);
            var mid = await svc.GetPlayerById(p.Id);
            Assert.Equal(PlayerActionEnum.FIGHT, mid.Action);

            await svc.ResetAction(p.Id);
            var end = await svc.GetPlayerById(p.Id);
            Assert.Equal(PlayerActionEnum.NONE, end.Action);
        }

        [Fact]
        public async Task AddGold_And_RemoveGold_Behaviour()
        {
            using var ctx = CreateInMemoryContext();
            var svc = new PlayerService(ctx);

            var p = await svc.CreatePlayer("gold", "g", "h");
            await svc.AddGold(p.Id, 50);

            var after = await svc.GetPlayerById(p.Id);
            Assert.Equal(50, after.Gold);

            var cannot = await svc.RemoveGold(p.Id, 100);
            Assert.False(cannot);

            var ok = await svc.RemoveGold(p.Id, 20);
            Assert.True(ok);

            var final = await svc.GetPlayerById(p.Id);
            Assert.Equal(30, final.Gold);
        }

        [Fact]
        public async Task HealPlayer_TakeDamage_IsDead()
        {
            using var ctx = CreateInMemoryContext();
            var svc = new PlayerService(ctx);

            var p = await svc.CreatePlayer("hp", "h", "x");
            await svc.TakeDamage(p.Id, 80);
            var mid = await svc.GetPlayerById(p.Id);
            Assert.True(mid.Health <= mid.MaxHealth);
            Assert.False(await svc.IsDead(p.Id));

            await svc.TakeDamage(p.Id, 1000);
            Assert.True(await svc.IsDead(p.Id));

            await svc.HealPlayer(p.Id, 500);
            var after = await svc.GetPlayerById(p.Id);
            Assert.InRange(after.Health, 0, after.MaxHealth);
        }

        [Fact]
        public async Task Inventory_Add_And_RemoveArtifact()
        {
            using var ctx = CreateInMemoryContext();
            var svc = new PlayerService(ctx);

            var p = await svc.CreatePlayer("inv", "i", "h");
            var artifact = new Artifact { Name = "Ring", Description = "r", Rarity = RarityEnum.COMMON };
            await svc.AddArtifactToInventory(p.Id, artifact);

            var withInv = await svc.GetPlayerById(p.Id);
            Assert.NotNull(withInv.Inventory);
            Assert.Single(withInv.Inventory);
            var added = withInv.Inventory.First();

            var removedFalse = await svc.RemoveArtifactFromInventory(p.Id, 99999);
            Assert.False(removedFalse);

            var removedTrue = await svc.RemoveArtifactFromInventory(p.Id, added.Id);
            Assert.True(removedTrue);

            var final = await svc.GetPlayerById(p.Id);
            Assert.Empty(final.Inventory);
        }

        [Fact]
        public async Task GetPlayerStats_Returns_CorrectDto()
        {
            using var ctx = CreateInMemoryContext();
            var svc = new PlayerService(ctx);

            var player = new Player
            {
                Username = "statUser",
                Email = "s@e",
                PasswordHash = "h",
                Level = 3,
                ExperiencePoints = 150,
                LevelCap = 200,
                Gold = 77,
                Health = 80,
                MaxHealth = 100,
                Attack = 12,
                Defense = 4,
                Inventory = new List<Artifact>()
            };

            player.HighScore = new HighScore { Score = 999, DateAchieved = DateTime.UtcNow };

            ctx.Players.Add(player);

            var completedDungeon = new Dungeon { Name = "C1", Description = "d" };
            ctx.Dungeons.Add(completedDungeon);

            var gh = new GameHistory
            {
                Player = player,
                CompletedDungeons = new List<Dungeon> { completedDungeon }
            };
            ctx.GameHistories.Add(gh);

            ctx.SaveChanges();

            var dto = await svc.GetPlayerStats(player.Id);
            Assert.NotNull(dto);
            Assert.Equal(player.Username, dto.Username);
            Assert.Equal(player.Level, dto.Level);
            Assert.Equal(player.Gold, dto.Gold);
            Assert.Equal(1, dto.TotalDungeonsCompleted);
            Assert.Equal(999, dto.HighestScore);
        }
    }
}