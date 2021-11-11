using Microsoft.EntityFrameworkCore;
using MoviesApp.Models;

namespace MoviesApp.Data
{
    public class MoviesContext : DbContext
    {
        public MoviesContext (DbContextOptions<MoviesContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Movie> Movies { get; set; }
        public virtual DbSet<Artist> Artists { get; set; }
        public virtual DbSet<ArtistsMovie> ArtistsMovies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Cyrillic_General_CI_AS");   
            modelBuilder.Entity<ArtistsMovie>(entity =>
            {
                entity.HasKey(e => new { e.ArtistId, e.MovieId });
                entity.HasOne(d => d.Movie)
                    .WithMany(p => p.ArtistsMovie)
                    .HasForeignKey(d => d.MovieId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ArtistsMovies_Movies");

                entity.HasOne(d => d.Artist)
                    .WithMany(p => p.ArtistsMovie)
                    .HasForeignKey(d => d.ArtistId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ArtistsMovies_Artists");
            });
        }
    }
}