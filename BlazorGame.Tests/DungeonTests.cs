using SharedModels.Enum;
using SharedModels.Model;
using Xunit;

namespace BlazorGame.Tests
{
    public class DungeonTests
    {
        [Fact]
        public void ExploreDungeon_ShouldSetIsExploredTrue()
        {
            var dungeon = new Dungeon();
            dungeon.ExploreDungeon();
            Assert.True(dungeon.IsExplored);
        }
    }
}