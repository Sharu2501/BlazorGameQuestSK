using SharedModels.Enum;
using SharedModels.Model;
using Xunit;

namespace BlazorGame.Tests
{
    public class RoomTests
    {
        [Fact]
        public void Explore_ShouldSetIsExploredTrue()
        {
            var player = new Player()
            {
                PlayerId = 1,
                Level = 5,
                Username = "Player1",
                HighScore = 1000
            };

            var room = new Room()
            {
                Id = 1,
                Name = "Chambre des monstres",
                Level = 1,
                Description = "Une chambre sombre remplie de monstres.",
                ExperienceGained = 50,
                GoldGained = 20,
                DifficultyLevel = DifficultyLevelEnum.MEDIUM,
                Player = player
            };

            room.Explore();

            Assert.True(room.IsExplored);
        }

        [Fact]
        public void RoomToString_ShouldIncludePlayerInfos()
        {
            var player = new Player()
            {
                PlayerId = 2,
                Level = 3,
                Username = "Mage",
                HighScore = 500
            };
            
            var room = new Room()
            {
                Player = player
            };
            var result = room.ToString();

            Assert.Contains("Player", result);
            Assert.Contains(player.Username, result);
        }
    }
}