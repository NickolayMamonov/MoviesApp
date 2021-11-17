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

       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ArtistsMovie>(entity =>
            {
                entity.HasKey(e => new { e.ArtistId, e.MovieId });
            });
        }
        public virtual DbSet<Movie> Movies { get; set; }
        public virtual DbSet<Artist> Artists { get; set; }
        public virtual DbSet<ArtistsMovie> ArtistsMovies { get; set; }
    }
}