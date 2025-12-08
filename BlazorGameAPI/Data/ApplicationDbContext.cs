using Microsoft.EntityFrameworkCore;
using SharedModels.Model;
using SharedModels.Enum;

namespace BlazorGameAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Monster> Monsters { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Dungeon> Dungeons { get; set; }
        public DbSet<Artifact> Artifacts { get; set; }
        public DbSet<GameSession> GameSessions { get; set; }
        public DbSet<GameHistory> GameHistories { get; set; }
        public DbSet<HighScore> HighScores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Monster>()
                .HasKey(m => m.IdMonster);

            modelBuilder.Entity<Dungeon>()
                .HasKey(d => d.IdDungeon);

            modelBuilder.Entity<GameSession>()
                .HasKey(gs => gs.SessionId);

            modelBuilder.Entity<User>()
                .HasDiscriminator(u => u.UserType)
                .HasValue<User>(UserTypeEnum.DEFAULT)
                .HasValue<Player>(UserTypeEnum.PLAYER)
                .HasValue<Admin>(UserTypeEnum.ADMIN);

            modelBuilder.Entity<Player>()
                .HasMany(p => p.Inventory)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Player>()
                .HasOne(p => p.HighScore)
                .WithOne()
                .HasForeignKey<HighScore>("PlayerId")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Dungeon>()
                .HasMany(d => d.Rooms)
                .WithOne()
                .HasForeignKey("DungeonId")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Dungeon>()
                .HasOne(d => d.Artifact)
                .WithOne()
                .HasForeignKey<Dungeon>("ArtifactId")
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Room>()
                .HasOne(r => r.Monster)
                .WithOne()
                .HasForeignKey<Room>("MonsterId")
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<GameHistory>()
                .HasOne(gh => gh.Player)
                .WithMany(p => p.GameHistories)
                .HasForeignKey("PlayerId")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GameHistory>()
                .HasMany(gh => gh.CompletedDungeons)
                .WithMany()
                .UsingEntity(j => j.ToTable("GameHistoryDungeons"));

            modelBuilder.Entity<GameSession>()
                .HasIndex(gs => gs.PlayerId);

            modelBuilder.Entity<GameSession>()
                .HasIndex(gs => gs.IsActive);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username);

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<User>()
                .Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<User>()
                .Property(u => u.PasswordHash)
                .IsRequired();

            modelBuilder.Entity<Monster>()
                .Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Dungeon>()
                .Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Room>()
                .Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Artifact>()
                .Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}