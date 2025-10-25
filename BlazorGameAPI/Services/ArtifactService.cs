using BlazorGameAPI.Data;
using SharedModels.Enum;
using SharedModels.Model;

namespace BlazorGameAPI.Services
{
    public class ArtifactService
    {
        private readonly ApplicationDbContext _context;

        public ArtifactService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Crée un nouvel artefact.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="rarity"></param>
        /// <returns></returns>
        public Task<Artifact> CreateArtifact(string name, string description, SharedModels.Enum.RarityEnum rarity)
        {
            var artifact = new Artifact
            {
                Name = name,
                Description = description,
                Rarity = rarity
            };

            _context.Artifacts.Add(artifact);
            _context.SaveChanges();

            return Task.FromResult(artifact);
        }

        /// <summary>
        /// Récupère un artefact par son ID.
        /// </summary>
        /// <param name="artifactId"></param>
        /// <returns></returns>
        public Task<Artifact?> GetArtifactById(int artifactId)
        {
            Artifact? artifact = _context.Artifacts.Find(artifactId);
            return Task.FromResult(artifact);
        }

        /// <summary>
        /// Récupère tous les artefacts.
        /// </summary>
        /// <returns></returns>
        public Task<List<Artifact>> GetAllArtifacts()
        {
            List<Artifact> artifacts = _context.Artifacts.ToList();
            return Task.FromResult(artifacts);
        }

        /// <summary>
        /// Supprime un artefact par son ID.
        /// </summary>
        /// <param name="artifactId"></param>
        /// <returns></returns>
        public Task<bool> DeleteArtifact(int artifactId)
        {
            var artifact = _context.Artifacts.Find(artifactId);
            if (artifact == null)
            {
                return Task.FromResult(false);
            }

            _context.Artifacts.Remove(artifact);
            _context.SaveChanges();
            return Task.FromResult(true);
        }

        /// <summary>
        /// Récupère tous les artefacts d'une certaine rareté.
        /// </summary>
        /// <param name="rarity"></param>
        /// <returns></returns>
        public Task<List<Artifact>> GetArtifactsByRarity(RarityEnum rarity)
        {
            List<Artifact> artifacts = _context.Artifacts
                .Where(a => a.Rarity == rarity)
                .ToList();
            return Task.FromResult(artifacts);
        }
        /// <summary>
        /// Récupère un artefact aléatoire.
        /// </summary>
        /// <returns></returns>
        public Task<Artifact?> GetRandomArtifact()
        {
            var artifacts = _context.Artifacts.ToList();
            if (artifacts.Count == 0)
            {
                return Task.FromResult<Artifact?>(null);
            }

            var random = new Random();
            int index = random.Next(artifacts.Count);
            return Task.FromResult<Artifact?>(artifacts[index]);
        }

        /// <summary>
        /// Récupère un artefact aléatoire d'une certaine rareté.
        /// </summary>
        /// <param name="rarity"></param>
        /// <returns></returns>
        public Task<Artifact?> GetRandomArtifactByRarity(RarityEnum rarity)
        {
            var artifacts = _context.Artifacts
                .Where(a => a.Rarity == rarity)
                .ToList();

            if (artifacts.Count == 0)
            {
                return Task.FromResult<Artifact?>(null);
            }

            var random = new Random();
            int index = random.Next(artifacts.Count);
            return Task.FromResult<Artifact?>(artifacts[index]);
        }
    }
}