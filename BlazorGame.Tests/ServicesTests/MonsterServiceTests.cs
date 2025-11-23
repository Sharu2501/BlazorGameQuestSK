using BlazorGameAPI.Services;
using SharedModels.Enum;
using SharedModels.Model;

namespace BlazorGame.Tests.ServicesTests
{
    public class MonsterServiceTests : TestBase
    {
        [Fact]
        public async Task CreateMonster_And_GetById_Works()
        {
            using var ctx = CreateInMemoryContext();
            var svc = new MonsterService(ctx);

            var created = await svc.CreateMonster("Grunt", 2, 80, 8, 4, MonsterTypeEnum.GOBLIN);
            Assert.NotNull(created);
            Assert.Equal(2, created.Level);

            var fetched = await svc.GetMonsterById(created.IdMonster);
            Assert.NotNull(fetched);
            Assert.Equal(created.IdMonster, fetched.IdMonster);
        }

        [Fact]
        public async Task GetAllMonsters_And_LevelRange_Filter()
        {
            using var ctx = CreateInMemoryContext();
            ctx.Monsters.AddRange(new List<Monster>
            {
                new Monster { Name = "M1", Level = 1, Health = 10, Attack = 1, Defense = 1, Type = MonsterTypeEnum.GOBLIN },
                new Monster { Name = "M2", Level = 5, Health = 60, Attack = 6, Defense = 3, Type = MonsterTypeEnum.TROLL },
                new Monster { Name = "M3", Level = 10, Health = 160, Attack = 20, Defense = 10, Type = MonsterTypeEnum.DRAGON }
            });
            ctx.SaveChanges();

            var svc = new MonsterService(ctx);
            var all = await svc.GetAllMonsters();
            Assert.Equal(3, all.Count);

            var range = await svc.GetMonstersByLevelRange(2, 8);
            Assert.Single(range);
            Assert.Equal(5, range.First().Level);
        }

        [Fact]
        public async Task GetRandomMonster_ReturnsNull_WhenEmpty_And_ReturnsOne_WhenPresent()
        {
            using var ctx = CreateInMemoryContext();
            var svc = new MonsterService(ctx);

            var none = await svc.GetRandomMonster(1, 10);
            Assert.Null(none);

            ctx.Monsters.Add(new Monster { Name = "RandM", Level = 3, Health = 40, Attack = 4, Defense = 2, Type = MonsterTypeEnum.BEAST });
            ctx.Monsters.Add(new Monster { Name = "RandM2", Level = 4, Health = 50, Attack = 5, Defense = 3, Type = MonsterTypeEnum.UNDEAD });
            ctx.SaveChanges();

            var some = await svc.GetRandomMonster(1, 4);
            Assert.NotNull(some);
            Assert.InRange(some.Level, 1, 4);
        }

        [Fact]
        public async Task UpdateMonster_ReturnsFalse_WhenMissing_And_UpdateWorks_WhenPresent()
        {
            using var ctx = CreateInMemoryContext();
            var svc = new MonsterService(ctx);

            var fake = new Monster { IdMonster = 9999, Name = "X" };
            var notFound = await svc.UpdateMonster(fake);
            Assert.False(notFound);

            var created = await svc.CreateMonster("OldName", 1, 30, 3, 1, MonsterTypeEnum.HUMANOID);
            created.Name = "NewName";
            created.Health = 99;

            var ok = await svc.UpdateMonster(created);
            Assert.True(ok);

            var fromDb = await ctx.Monsters.FindAsync(created.IdMonster);
            Assert.Equal("NewName", fromDb.Name);
            Assert.Equal(99, fromDb.Health);
        }

        [Fact]
        public async Task DeleteMonster_ReturnsFalse_WhenMissing_And_Deletes_WhenPresent()
        {
            using var ctx = CreateInMemoryContext();
            var svc = new MonsterService(ctx);

            var notFound = await svc.DeleteMonster(123456);
            Assert.False(notFound);

            var created = await svc.CreateMonster("ToDie", 1, 10, 1, 1, MonsterTypeEnum.GOBLIN);
            var ok = await svc.DeleteMonster(created.IdMonster);
            Assert.True(ok);

            var after = await ctx.Monsters.FindAsync(created.IdMonster);
            Assert.Null(after);
        }

        [Fact]
        public async Task GenerateMonsterForLevel_Returns_Monster_With_Specified_Type_And_Level()
        {
            using var ctx = CreateInMemoryContext();
            var svc = new MonsterService(ctx);

            var m = await svc.GenerateMonsterForLevel(6, DifficultyLevelEnum.MEDIUM);
            Assert.NotNull(m);
            Assert.InRange(m.Level, 5, 7);
            Assert.Equal(DifficultyLevelEnum.MEDIUM, m.Type switch
            {
                MonsterTypeEnum.GOBLIN or MonsterTypeEnum.TROLL => DifficultyLevelEnum.EASY,
                MonsterTypeEnum.UNDEAD or MonsterTypeEnum.BEAST => DifficultyLevelEnum.MEDIUM,
                MonsterTypeEnum.HUMANOID => DifficultyLevelEnum.HARD,
                MonsterTypeEnum.DRAGON => DifficultyLevelEnum.EXTREME,
                _ => DifficultyLevelEnum.MEDIUM
            });
            Assert.True(m.Health > 0);
            Assert.True(m.Attack > 0);
            Assert.True(m.Defense > 0);
        }
    }
}