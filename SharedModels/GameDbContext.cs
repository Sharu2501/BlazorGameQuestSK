using Microsoft.EntityFrameworkCore;

/// <summary>
/// DbContext pour le jeu, gérant la connexion à la base de données et les ensembles d'entités.
/// </summary>
namespace SharedModels
{
    public class GameDbContext : DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

        public DbSet<Room> Rooms { get; set; }
    }
}