using BlazorGameAPI.Services;
using BlazorGameAPI.Data;
using SharedModels.Model;
using SharedModels.Enum;

namespace BlazorGame.Tests.ServicesTests
{
    public class CombatServiceTests : TestBase
    {
        [Fact]
        public async Task CalculateDamage_ReturnsNonNegative()
        {
            using var ctx = CreateInMemoryContext();
            var playerService = new PlayerService(ctx);
            var svc = new CombatService(ctx, playerService);

            var dmg = await svc.CalculateDamage(10, 5);
            Assert.IsType<int>(dmg);
            Assert.True(dmg >= 0);
        }

        [Fact]
        public async Task PlayerAttacksMonster_ReturnsFalse_WhenMissing_And_DecreasesMonsterHealth_WhenPresent()
        {
            using var ctx = CreateInMemoryContext();
            var playerService = new PlayerService(ctx);
            var svc = new CombatService(ctx, playerService);

            var missing = await svc.PlayerAttacksMonster(9999, 8888);
            Assert.False(missing.Success);

            var player = new Player { Username = "P", Level = 1, Attack = 10, Defense = 2, Health = 50, MaxHealth = 50 };
            var monster = new Monster { Name = "M", Level = 1, Health = 40, Attack = 5, Defense = 1, Type = MonsterTypeEnum.GOBLIN };
            ctx.Players.Add(player);
            ctx.Monsters.Add(monster);
            ctx.SaveChanges();

            var before = ctx.Monsters.Find(monster.IdMonster).Health;
            var ok = await svc.PlayerAttacksMonster(player.Id, monster.IdMonster);
            Assert.True(ok.Success);

            var after = ctx.Monsters.Find(monster.IdMonster).Health;
            Assert.True(after <= before);
        }

        [Fact]
        public async Task MonsterAttacksPlayer_DecreasesPlayerHealth_WhenPresent()
        {
            using var ctx = CreateInMemoryContext();
            var playerService = new PlayerService(ctx);
            var svc = new CombatService(ctx, playerService);

            var player = new Player { Username = "P2", Level = 1, Attack = 5, Defense = 1, Health = 30, MaxHealth = 30 };
            var monster = new Monster { Name = "M2", Level = 1, Health = 20, Attack = 8, Defense = 2, Type = MonsterTypeEnum.TROLL };
            ctx.Players.Add(player);
            ctx.Monsters.Add(monster);
            ctx.SaveChanges();

            var before = ctx.Players.Find(player.Id).Health;
            var ok = await svc.MonsterAttacksPlayer(monster.IdMonster, player.Id);
            Assert.True(ok.Success);

            var after = ctx.Players.Find(player.Id).Health;
            Assert.True(after <= before);
        }

        [Fact]
        public async Task PlayerDefends_IncreasesDefense_And_ReturnsFalse_WhenMissing()
        {
            using var ctx = CreateInMemoryContext();
            var playerService = new PlayerService(ctx);
            var svc = new CombatService(ctx, playerService);

            var missing = await svc.PlayerDefends(9999);
            Assert.False(missing);

            var player = new Player { Username = "Def", Defense = 2, Health = 10, MaxHealth = 10 };
            ctx.Players.Add(player);
            ctx.SaveChanges();

            var before = ctx.Players.Find(player.Id).Defense;
            var ok = await svc.PlayerDefends(player.Id);
            Assert.True(ok);
            var after = ctx.Players.Find(player.Id).Defense;
            Assert.True(after >= before);
        }

        [Fact]
        public async Task PlayerHealsInCombat_RespectsMaxHealth_And_ReturnsFalse_WhenMissing()
        {
            using var ctx = CreateInMemoryContext();
            var playerService = new PlayerService(ctx);
            var svc = new CombatService(ctx, playerService);

            var missing = await svc.PlayerHealsInCombat(9999, 10);
            Assert.False(missing);

            var player = new Player { Username = "Healer", Health = 5, MaxHealth = 20 };
            ctx.Players.Add(player);
            ctx.SaveChanges();

            var before = ctx.Players.Find(player.Id).Health;
            var ok = await svc.PlayerHealsInCombat(player.Id, 8);
            Assert.True(ok);
            var after = ctx.Players.Find(player.Id).Health;
            Assert.InRange(after, before, player.MaxHealth);
        }

        [Fact]
        public async Task PlayerFlees_HigherLevel_IncreasesChance()
        {
            using var ctx = CreateInMemoryContext();
            var playerService = new PlayerService(ctx);
            var svc = new CombatService(ctx, playerService);

            var low = new Player { Username = "Low", Level = 1, Health = 10, MaxHealth = 10 };
            var high = new Player { Username = "High", Level = 50, Health = 10, MaxHealth = 10 };
            ctx.Players.AddRange(low, high);
            ctx.SaveChanges();

            var canLow = await svc.PlayerFlees(low.Id);
            Assert.IsType<bool>(canLow);

            var canHigh = await svc.PlayerFlees(high.Id);
            Assert.IsType<bool>(canHigh);
        }

        [Fact]
        public async Task IsMonsterDefeated_And_IsPlayerDefeated_Works()
        {
            using var ctx = CreateInMemoryContext();
            var playerService = new PlayerService(ctx);
            var svc = new CombatService(ctx, playerService);

            var deadMonster = new Monster { Name = "Dead", Health = 0 };
            var aliveMonster = new Monster { Name = "Alive", Health = 10 };
            ctx.Monsters.AddRange(deadMonster, aliveMonster);

            var player = new Player { Username = "AliveP", Health = 1, MaxHealth = 10 };
            var deadPlayer = new Player { Username = "DeadP", Health = 0, MaxHealth = 10 };
            ctx.Players.AddRange(player, deadPlayer);
            ctx.SaveChanges();

            Assert.True(await svc.IsMonsterDefeated(deadMonster.IdMonster));
            Assert.False(await svc.IsMonsterDefeated(aliveMonster.IdMonster));

            Assert.False(await svc.IsPlayerDefeated(player.Id));
            Assert.True(await svc.IsPlayerDefeated(deadPlayer.Id));
        }

        [Fact]
        public async Task ResolveCombatVictory_IncreasesPlayerRewards_And_ExploresRoom_ResolveCombatDefeat_LosesGold_And_RestoresHealth()
        {
            using var ctx = CreateInMemoryContext();
            var playerService = new PlayerService(ctx);
            var svc = new CombatService(ctx, playerService);

            var player = new Player { Username = "Winner", ExperiencePoints = 0, Gold = 10, Health = 20, MaxHealth = 50 };
            ctx.Players.Add(player);
            var room = new Room { Name = "R", ExperienceGained = 100, GoldGained = 50, DifficultyLevel = DifficultyLevelEnum.MEDIUM, IsExplored = false };
            ctx.Rooms.Add(room);
            ctx.SaveChanges();

            await svc.ResolveCombatVictory(player.Id, room.Id);
            var pAfter = ctx.Players.Find(player.Id);
            var rAfter = ctx.Rooms.Find(room.Id);

            Assert.True(pAfter.ExperiencePoints >= 0);
            Assert.True(pAfter.Gold >= 10);
            Assert.True(rAfter.IsExplored);

            var victim = new Player { Username = "Victim", Gold = 100, Health = 10, MaxHealth = 100 };
            ctx.Players.Add(victim);
            var hardRoom = new Room { Name = "HR", DifficultyLevel = DifficultyLevelEnum.HARD };
            ctx.Rooms.Add(hardRoom);
            ctx.SaveChanges();

            await svc.ResolveCombatDefeat(victim.Id, hardRoom.Id);
            var vAfter = ctx.Players.Find(victim.Id);
            Assert.True(vAfter.Gold < 100);
            Assert.Equal((int)(vAfter.MaxHealth * 0.2), vAfter.Health);
        }
    }
}