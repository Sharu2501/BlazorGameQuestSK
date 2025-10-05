using Microsoft.EntityFrameworkCore;

namespace SharedModels
{
    public class GameDbContext : DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

        public DbSet<Salle> Salles { get; set; }
    }
}