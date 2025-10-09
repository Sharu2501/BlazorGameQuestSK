using SharedModels;
using Xunit;

namespace BlazorGame.Tests
{
    public class ArtifactTests
    {
        [Fact]
        public void DefaultConstructor_ShouldInitializeProperties()
        {
            var artifact = new Artifact();
            Assert.Equal(0, artifact.Id);
            Assert.Equal("default", artifact.Name);
            Assert.Equal("default", artifact.Description);
            Assert.Equal(RarityEnum.COMMON, artifact.Rarity);
        }

        [Fact]
        public void ParameterizedConstructor_ShouldSetProperties()
        {
            var artifact = new Artifact(1, "Sword", "Sharp sword", RarityEnum.EPIC);
            Assert.Equal(1, artifact.Id);
            Assert.Equal("Sword", artifact.Name);
            Assert.Equal("Sharp sword", artifact.Description);
            Assert.Equal(RarityEnum.EPIC, artifact.Rarity);
        }
    }
}