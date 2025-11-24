using Xunit;
using Microsoft.EntityFrameworkCore;
using BlazorGameAPI.Services;
using SharedModels.Model;
using SharedModels.Enum;
using System.Threading.Tasks;
using System.Linq;
using BlazorGameAPI.Data;

namespace BlazorGameAPI.Tests.ModelsTests
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

            var player = await service.CreatePlayer("TestPlayer", "test@gmail.com", "hashedpassword");

            Assert.NotNull(player);
            Assert.Equal("TestPlayer", player.Username);
            Assert.Equal(1, player.Level);
            Assert.Equal(UserTypeEnum.PLAYER, player.UserType);
            Assert.Equal(100, player.MaxHealth);
        }
    }
}