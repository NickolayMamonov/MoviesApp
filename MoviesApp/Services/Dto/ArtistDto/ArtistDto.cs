using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MoviesApp.Models;

namespace MoviesApp.Services.Dto
{
    public class ArtistDto
    {
        public int? Id { get; set; }
        
        [Required]
        public string FirstName { get; set; }
        
        [Required]
        public string LastName { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }

        public virtual ICollection<ArtistsMovie> ArtistsMovies { get; set; }
        
        public ICollection<string> SelectOptions { get; set; }
    }
}