using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MoviesApp.Models;

namespace MoviesApp.ViewModels
{
    public class InputMovieViewModel
    {
        public InputMovieViewModel()
        {
            ArtistsMovies = new HashSet<ArtistsMovie>();
            Artists = new HashSet<Artist>();
        }
        public string Title { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }
        public string Genre { get; set; }
        public decimal Price { get; set; }
        public ICollection<ArtistsMovie> ArtistsMovies { get; set; }
        public ICollection<Artist> Artists { get; set; }
    }
}