using BlazorGameAPI.Data;
using BlazorGameAPI.Services;
using Microsoft.AspNetCore.Mvc;
using SharedModels.Enum;
using SharedModels.Model;
using BlazorGameAPI.Controllers;

namespace BlazorGame.Tests.ControllersTests
{
    public class CombatControllerTests : TestBase
    {
        private CombatController CreateController(ApplicationDbContext ctx)
        {
            var playerService = new PlayerService(ctx);
            var combatService = new CombatService(ctx, playerService);
            return new CombatController(combatService);
        }

        [Fact]
        public async Task Attack_ReturnsOk_WithAttackResult()
        {
            using var ctx = CreateInMemoryContext();
            var controller = CreateController(ctx);

            var player = new Player { Username = "p1", Level = 3, Attack = 10, Defense = 5, Health = 30, MaxHealth = 30 };
            var monster = new Monster { Name = "Gob", Level = 2, Attack = 5, Defense = 3, Health = 20 };
            ctx.Players.Add(player);
            ctx.Monsters.Add(monster);
            ctx.SaveChanges();

            var req = new AttackRequest { PlayerId = player.Id, MonsterId = monster.IdMonster };

            var result = await controller.Attack(req);

            var ok = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsType<AttackResult>(ok.Value);
            Assert.True(data.Success);
        }

        [Fact]
        public async Task MonsterAttack_ReturnsOk_WithAttackResult()
        {
            using var ctx = CreateInMemoryContext();
            var controller = CreateController(ctx);

            var player = new Player { Username = "p1", Level = 3, Attack = 10, Defense = 5, Health = 30, MaxHealth = 30 };
            var monster = new Monster { Name = "Troll", Level = 4, Attack = 12, Defense = 4, Health = 25 };
            ctx.Players.Add(player);
            ctx.Monsters.Add(monster);
            ctx.SaveChanges();

            var req = new MonsterAttackRequest { PlayerId = player.Id, MonsterId = monster.IdMonster };

            var result = await controller.MonsterAttack(req);

            var ok = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsType<AttackResult>(ok.Value);
            Assert.True(data.Success);
        }

        [Fact]
        public async Task Defend_ReturnsOk_WithBool()
        {
            using var ctx = CreateInMemoryContext();
            var controller = CreateController(ctx);

            var player = new Player { Username = "p1", Defense = 5, Health = 30, MaxHealth = 30 };
            ctx.Players.Add(player);
            ctx.SaveChanges();

            var req = new DefendRequest { PlayerId = player.Id };

            var result = await controller.Defend(req);

            var ok = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<bool>(ok.Value);
            Assert.True(value);
        }

        [Fact]
        public async Task Flee_ReturnsOk_WithBool()
        {
            using var ctx = CreateInMemoryContext();
            var controller = CreateController(ctx);

            var player = new Player { Username = "p1", Level = 5, Health = 30, MaxHealth = 30 };
            ctx.Players.Add(player);
            ctx.SaveChanges();

            var req = new FleeRequest { PlayerId = player.Id };

            var actionResult = await controller.Flee(req);
            var ok = Assert.IsType<OkObjectResult>(actionResult.Result);
            var value = Assert.IsType<bool>(ok.Value);
            // Peut être true ou false selon le jet, on vérifie juste que ça renvoie bien un bool
        }

        [Fact]
        public async Task Victory_CallsService_AndReturnsOk()
        {
            using var ctx = CreateInMemoryContext();
            var controller = CreateController(ctx);

            var player = new Player { Username = "p1", Level = 1, ExperiencePoints = 0, Gold = 0, Health = 20, MaxHealth = 20 };
            var room = new Room
            {
                Name = "Salle",
                DifficultyLevel = DifficultyLevelEnum.MEDIUM,
                ExperienceGained = 100,
                GoldGained = 50,
                IsExplored = false
            };
            ctx.Players.Add(player);
            ctx.Rooms.Add(room);
            ctx.SaveChanges();

            var req = new VictoryRequest { PlayerId = player.Id, RoomId = room.Id };

            var result = await controller.Victory(req);

            var ok = Assert.IsType<OkResult>(result);

            var updatedPlayer = await ctx.Players.FindAsync(player.Id);
            var updatedRoom = await ctx.Rooms.FindAsync(room.Id);

            Assert.True(updatedRoom!.IsExplored);
            Assert.True(updatedPlayer!.ExperiencePoints > 0);
            Assert.True(updatedPlayer.Gold > 0);
        }
    }
}