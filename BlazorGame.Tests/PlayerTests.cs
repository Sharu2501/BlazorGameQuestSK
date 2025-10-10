using SharedModels;
using Xunit;

namespace BlazorGame.Tests
{
    public class PlayerTests
    {
        [Fact]
        public void Constructor_DefaultValues_ShouldInitializeCorrectly()
        {
            var player = new Player();

            // Assertions pour les valeurs par défaut
            Assert.Equal(0, player.PlayerId);
            Assert.Equal(0, player.Level);
            Assert.Equal("default", player.Username);
            Assert.Equal(0, player.HighScore);
            Assert.Equal(PlayerActionEnum.NONE, player.Action);
        }

        [Fact]
        public void Constructor_CustomValues_ShouldInitializeCorrectly()
        {
            var player = new Player()
            {
                PlayerId = 1,
                Level = 5,
                Username = "TestPlayer",
                HighScore = 100,
                Action = PlayerActionEnum.NONE
            };

            // Assertion pour le joueur avec l'id 1, le niveau 5, le nom d'utilisateur "TestPlayer" et un score de 100
            Assert.Equal(1, player.PlayerId);
            Assert.Equal(5, player.Level);
            Assert.Equal("TestPlayer", player.Username);
            Assert.Equal(100, player.HighScore);
            Assert.Equal(PlayerActionEnum.NONE, player.Action);
        }

        [Fact]
        public void AddExperience_ShouldIncreaseExperienceAndLevelUp()
        {
            var player = new Player()
            {
                PlayerId = 1,
                Level = 1,
                Username = "Test",
                HighScore = 0,
                Action = PlayerActionEnum.NONE,
                ExperiencePoints = 90,
                LevelCap = 100
            };

            // Réalisation d'une action
            player.AddExperience(15); // total 105 donc passage au niveau suivant

            // Assertion pour vérifier l'augmentation de l'expérience et le niveau
            Assert.Equal(2, player.Level);
            Assert.Equal(5, player.ExperiencePoints); // 105 - 100 = 5
        }

        [Fact]
        public void ChangeAction_ShouldUpdateAction()
        {
            var player = new Player();

            // Le joueur choisit de se battre comme action
            player.ChangeAction(PlayerActionEnum.FIGHT);

            // Assertion pour vérifier que l'action a été mise à jour
            Assert.Equal(PlayerActionEnum.FIGHT, player.Action);
        }

        [Fact]
        public void ResetAction_ShouldSetActionToNone()
        {
            var player = new Player();
            // Le joueur choisit de se soigner comme action
            player.ChangeAction(PlayerActionEnum.HEAL);

            // Le joueur réinitialise son action
            player.ResetAction();

            // Assertion pour vérifier que l'action est revenue à NONE
            Assert.Equal(PlayerActionEnum.NONE, player.Action);
        }
    }
}