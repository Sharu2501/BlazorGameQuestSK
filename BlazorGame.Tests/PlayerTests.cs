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
            Assert.Equal(0, player.GetPlayerId());
            Assert.Equal(0, player.GetLevel());
            Assert.Equal("default", player.GetUsername());
            Assert.Equal(0, player.GetHighScore());
            Assert.Equal(PlayerActionEnum.NONE, player.GetAction());
        }

        [Fact]
        public void Constructor_CustomValues_ShouldInitializeCorrectly()
        {
            var player = new Player(1, 5, "TestPlayer", 100);

            // Assertion pour le joueur avec l'id 1, le niveau 5, le nom d'utilisateur "TestPlayer" et un score de 100
            Assert.Equal(1, player.GetPlayerId());
            Assert.Equal(5, player.GetLevel());
            Assert.Equal("TestPlayer", player.GetUsername());
            Assert.Equal(100, player.GetHighScore());
            Assert.Equal(PlayerActionEnum.NONE, player.GetAction());
        }

        [Fact]
        public void AddExperience_ShouldIncreaseExperienceAndLevelUp()
        {
            var player = new Player(1, 1, "Test", 0, PlayerActionEnum.NONE, experiencePoints: 90, levelCap: 100);

            // Réalisation d'une action
            player.AddExperience(15); // total 105 donc passage au niveau suivant

            // Assertion pour vérifier l'augmentation de l'expérience et le niveau
            Assert.Equal(2, player.GetLevel());
            Assert.Equal(5, player.GetExperiencePoints()); // 105 - 100 = 5
        }

        [Fact]
        public void ChangeAction_ShouldUpdateAction()
        {
            var player = new Player();

            // Le joueur choisit de se battre comme action
            player.ChangeAction(PlayerActionEnum.FIGHT);

            // Assertion pour vérifier que l'action a été mise à jour
            Assert.Equal(PlayerActionEnum.FIGHT, player.GetAction());
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
            Assert.Equal(PlayerActionEnum.NONE, player.GetAction());
        }
    }
}