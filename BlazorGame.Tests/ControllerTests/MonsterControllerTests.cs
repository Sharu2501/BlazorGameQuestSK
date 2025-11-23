using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using BlazorGameAPI.Data;
using SharedModels.Model;

namespace BlazorGame.Tests.ControllersTests
{
    public class MonsterControllerTests : TestBase
    {
        [Fact]
        public async Task GetMonsters_ReturnsOk_WithList()
        {
            using var ctx = CreateInMemoryContext();
            ctx.Monsters.Add(new Monster { Name = "M1", Level = 1, Health = 10 });
            ctx.Monsters.Add(new Monster { Name = "M2", Level = 2, Health = 20 });
            ctx.SaveChanges();

            var controller = new MonsterController(ctx);
            var result = await controller.GetMonsters();

            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<System.Collections.Generic.List<Monster>>(ok.Value);
            Assert.Equal(2, list.Count);
        }

        [Fact]
        public async Task CreateMonster_ReturnsCreated_And_Persists()
        {
            using var ctx = CreateInMemoryContext();
            var controller = new MonsterController(ctx);

            var monster = new Monster { Name = "NewM", Level = 3, Health = 30, Attack = 5, Defense = 2 };
            var res = await controller.CreateMonster(monster);

            var created = Assert.IsType<CreatedAtActionResult>(res);
            var returned = Assert.IsType<Monster>(created.Value);
            Assert.Equal("NewM", returned.Name);

            var fromDb = await ctx.Monsters.FindAsync(returned.IdMonster);
            Assert.NotNull(fromDb);
            Assert.Equal("NewM", fromDb.Name);
        }
    }
}