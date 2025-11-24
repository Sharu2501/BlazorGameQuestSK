using BlazorGameAPI.Services;
using SharedModels.Enum;
using SharedModels.Model;

namespace BlazorGame.Tests.ServicesTests
{
    public class ArtifactServiceTests : TestBase
    {
        [Fact]
        public async Task CreateArtifact_SavesArtifact()
        {
            using var ctx = CreateInMemoryContext();
            var svc = new ArtifactService(ctx);

            var artifact = await svc.CreateArtifact("Excalibur", "Legendary blade", RarityEnum.LEGENDARY);

            Assert.NotNull(artifact);
            Assert.Equal("Excalibur", artifact.Name);

            var fromDb = await ctx.Artifacts.FindAsync(artifact.Id);
            Assert.NotNull(fromDb);
            Assert.Equal(RarityEnum.LEGENDARY, fromDb.Rarity);
        }

        [Fact]
        public async Task GetArtifactById_ReturnsNull_WhenMissing()
        {
            using var ctx = CreateInMemoryContext();
            var svc = new ArtifactService(ctx);

            var none = await svc.GetArtifactById(9999);
            Assert.Null(none);
        }

        [Fact]
        public async Task GetAllArtifacts_ReturnsAll()
        {
            using var ctx = CreateInMemoryContext();
            ctx.Artifacts.AddRange(new List<Artifact>
            {
                new Artifact { Name = "A1", Description = "d1", Rarity = RarityEnum.COMMON },
                new Artifact { Name = "A2", Description = "d2", Rarity = RarityEnum.RARE }
            });
            ctx.SaveChanges();

            var svc = new ArtifactService(ctx);
            var all = await svc.GetAllArtifacts();

            Assert.Equal(2, all.Count);
            Assert.Contains(all, a => a.Name == "A1");
            Assert.Contains(all, a => a.Name == "A2");
        }

        [Fact]
        public async Task GetArtifactsByRarity_FiltersCorrectly()
        {
            using var ctx = CreateInMemoryContext();
            ctx.Artifacts.AddRange(new[]
            {
                new Artifact { Name = "C1", Rarity = RarityEnum.COMMON },
                new Artifact { Name = "R1", Rarity = RarityEnum.RARE },
                new Artifact { Name = "C2", Rarity = RarityEnum.COMMON }
            });
            ctx.SaveChanges();

            var svc = new ArtifactService(ctx);
            var commons = await svc.GetArtifactsByRarity(RarityEnum.COMMON);

            Assert.Equal(2, commons.Count);
            Assert.All(commons, a => Assert.Equal(RarityEnum.COMMON, a.Rarity));
        }

        [Fact]
        public async Task DeleteArtifact_RemovesArtifact_And_ReturnsFalse_WhenMissing()
        {
            using var ctx = CreateInMemoryContext();
            var svc = new ArtifactService(ctx);

            var notFound = await svc.DeleteArtifact(9999);
            Assert.False(notFound);

            var a = new Artifact { Name = "ToDel", Description = "x", Rarity = RarityEnum.COMMON };
            ctx.Artifacts.Add(a);
            ctx.SaveChanges();

            var ok = await svc.DeleteArtifact(a.Id);
            Assert.True(ok);

            var after = await ctx.Artifacts.FindAsync(a.Id);
            Assert.Null(after);
        }

        [Fact]
        public async Task GetRandomArtifact_ReturnsNull_WhenEmpty_And_ReturnsOne_WhenPresent()
        {
            using var ctx = CreateInMemoryContext();
            var svc = new ArtifactService(ctx);

            var none = await svc.GetRandomArtifact();
            Assert.Null(none);

            ctx.Artifacts.Add(new Artifact { Name = "Rand", Rarity = RarityEnum.LEGENDARY });
            ctx.SaveChanges();

            var some = await svc.GetRandomArtifact();
            Assert.NotNull(some);
            Assert.Equal("Rand", some.Name);
        }

        [Fact]
        public async Task GetRandomArtifactByRarity_Behaviour()
        {
            using var ctx = CreateInMemoryContext();
            ctx.Artifacts.AddRange(new[]
            {
                new Artifact { Name = "Common1", Rarity = RarityEnum.COMMON },
                new Artifact { Name = "Rare1", Rarity = RarityEnum.RARE }
            });
            ctx.SaveChanges();

            var svc = new ArtifactService(ctx);

            var none = await svc.GetRandomArtifactByRarity(RarityEnum.LEGENDARY);
            Assert.Null(none);

            var one = await svc.GetRandomArtifactByRarity(RarityEnum.COMMON);
            Assert.NotNull(one);
            Assert.Equal(RarityEnum.COMMON, one.Rarity);
        }
    }
}