using Xunit;
using Microsoft.EntityFrameworkCore;
using BlazorGameAPI.Services;
using SharedModels.Model;
using SharedModels.Enum;
using System.Threading.Tasks;
using System.Linq;
using BlazorGameAPI.Data;

namespace BlazorGameAPI.Tests
{
    public class PlayerTests
    {
        private ApplicationDbContext GetInMemoryDbContext([System.Runtime.CompilerServices.CallerMemberName] string testName = "")
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{testName}")
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task CreatePlayer_ShouldAddPlayer()
        {
            var context = GetInMemoryDbContext();
            var service = new PlayerService(context);

            var player = await service.CreatePlayer("TestPlayer", "test@example.com", "hashedpassword");

            Assert.NotNull(player);
            Assert.Equal("TestPlayer", player.Username);
            Assert.Equal(1, player.Level);
            Assert.Equal(UserTypeEnum.PLAYER, player.UserType);
            Assert.Equal(100, player.MaxHealth);
        }

        [Fact]
        public async Task AddExperience_ShouldLevelUpPlayer()
        {
            var context = GetInMemoryDbContext();
            var service = new PlayerService(context);

            var player = await service.CreatePlayer("TestPlayer", "test@example.com", "hashedpassword");
            await service.AddExperience(player.Id, 150);

            var updatedPlayer = await service.GetPlayerById(player.Id);
            Assert.NotNull(updatedPlayer); 
            Assert.Equal(2, updatedPlayer.Level);
            Assert.Equal(50, updatedPlayer.ExperiencePoints);
        }

        [Fact]
        public async Task HealPlayer_ShouldNotExceedMaxHealth()
        {
            var context = GetInMemoryDbContext();
            var service = new PlayerService(context);

            var player = await service.CreatePlayer("TestPlayer", "test@example.com", "hashedpassword");
            await service.TakeDamage(player.Id, 50);
            await service.HealPlayer(player.Id, 100);

            var updatedPlayer = await service.GetPlayerById(player.Id);
            Assert.NotNull(updatedPlayer); 
            Assert.Equal(updatedPlayer.MaxHealth, updatedPlayer.Health);
        }

        [Fact]
        public async Task AddAndRemoveGold_ShouldWorkCorrectly()
        {
            var context = GetInMemoryDbContext();
            var service = new PlayerService(context);

            var player = await service.CreatePlayer("TestPlayer", "test@example.com", "hashedpassword");
            await service.AddGold(player.Id, 200);
            var updatedPlayer = await service.GetPlayerById(player.Id);
            Assert.NotNull(updatedPlayer); 
            Assert.Equal(200, updatedPlayer.Gold);

            bool removed = await service.RemoveGold(player.Id, 150);
            updatedPlayer = await service.GetPlayerById(player.Id);
            Assert.True(removed);
            Assert.NotNull(updatedPlayer); 
            Assert.Equal(50, updatedPlayer.Gold);
        }

        [Fact]
        public async Task AddArtifactToInventory_ShouldIncreaseCount()
        {
            var context = GetInMemoryDbContext();
            var service = new PlayerService(context);

            var player = await service.CreatePlayer("TestPlayer", "test@example.com", "hashedpassword");
            var artifact = new Artifact { Name = "Sword", Rarity = RarityEnum.RARE };
            await service.AddArtifactToInventory(player.Id, artifact);

            var updatedPlayer = await service.GetPlayerById(player.Id);
            Assert.NotNull(updatedPlayer); 
            Assert.Single(updatedPlayer.Inventory);
            Assert.Equal("Sword", updatedPlayer.Inventory.First().Name);
        }
    }
}