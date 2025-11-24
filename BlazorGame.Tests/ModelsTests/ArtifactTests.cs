using SharedModels.Enum;
using SharedModels.Model;
using Xunit;

namespace BlazorGame.Tests.ModelsTests
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
            var artifact = new Artifact()
            {
                Id = 1,
                Name = "Epée",
                Description = "Epée tranchante",
                Rarity = RarityEnum.EPIC
            };

            Assert.Equal(1, artifact.Id);
            Assert.Equal("Epée", artifact.Name);
            Assert.Equal("Epée tranchante", artifact.Description);
            Assert.Equal(RarityEnum.EPIC, artifact.Rarity);
        }
    }
}