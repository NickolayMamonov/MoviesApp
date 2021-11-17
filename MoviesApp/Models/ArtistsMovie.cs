namespace MoviesApp.Models
{
    public class ArtistsMovie
    {
        public  int MovieId { get; set; }
        public  int ArtistId { get; set; }
        
        public virtual Movie Movie { get; set; }
        public virtual Artist Artist { get; set; }
    }
}