using SharedModels;
using Xunit;

namespace BlazorGame.Tests
{
    public class MonsterTests
    {
        [Fact]
        public void DefaultConstructor_ShouldInitializeProperties()
        {
            var monster = new Monster();
            Assert.Equal(0, monster.IdMonster);
            Assert.Equal("default", monster.Name);
            Assert.Equal(1, monster.Level);
            Assert.Equal(100, monster.Health);
            Assert.Equal(10, monster.Attack);
            Assert.Equal(5, monster.Defense);
            Assert.Equal(MonsterTypeEnum.BEAST, monster.Type);
        }

        [Fact]
        public void ParameterizedConstructor_ShouldSetProperties()
        {
            var monster = new Monster(1, "Goblin", 2, 50, 12, 3, MonsterTypeEnum.GOBLIN);
            Assert.Equal(1, monster.IdMonster);
            Assert.Equal("Goblin", monster.Name);
            Assert.Equal(2, monster.Level);
            Assert.Equal(50, monster.Health);
            Assert.Equal(12, monster.Attack);
            Assert.Equal(3, monster.Defense);
            Assert.Equal(MonsterTypeEnum.GOBLIN, monster.Type);
        }
    }
}