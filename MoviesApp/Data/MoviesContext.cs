using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MoviesApp.Models;

namespace MoviesApp.Data
{
    public class MoviesContext : IdentityDbContext<ApplicationUser>
    {
        public MoviesContext (DbContextOptions<MoviesContext> options)
            : base(options)
        {
        }

       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ArtistsMovie>(entity =>
            {
                entity.HasKey(e => new { e.ArtistId, e.MovieId });
            });
        }
        public  DbSet<Movie> Movies { get; set; }
        public  DbSet<Artist> Artists { get; set; }
        public  DbSet<ArtistsMovie> ArtistsMovies { get; set; }
    }
}