using SharedModels;
using Xunit;

namespace BlazorGame.Tests
{
    public class RoomTests
    {
        [Fact]
        public void Explore_ShouldSetIsExploredTrue()
        {
            var player = new Player(1, 5, "Hero", 1000);
            var room = new Room(
                id: 1,
                name: "Chamber of Trials",
                level: 1,
                description: "A dark room filled with monsters.",
                monster: null,
                isExplored: false,
                experienceGained: 50,
                goldGained: 20,
                difficultyLevel: DifficultyLevelEnum.MEDIUM,
                player: player
            );

            room.Explore();

            Assert.True(room.IsExplored);
        }

        [Fact]
        public void RoomToString_ShouldIncludePlayerInfos()
        {
            var player = new Player(2, 3, "Mage", 500);
            var room = new Room(player: player);

            var result = room.ToString();

            Assert.Contains("Player", result);
            Assert.Contains(player.GetUsername(), result);
        }
    }
}