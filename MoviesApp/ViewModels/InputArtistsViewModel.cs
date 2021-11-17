using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MoviesApp.Models;

namespace MoviesApp.ViewModels
{
    public class InputArtistsViewModel
    {
        public InputArtistsViewModel()
        {
            ArtistsMovies = new HashSet<ArtistsMovie>();
            Movies = new HashSet<Movie>();
        }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime BirthdayDate { get; set; }
        
        public ICollection<ArtistsMovie> ArtistsMovies { get; set; }
        public ICollection<Movie> Movies { get; set; }
    }
}