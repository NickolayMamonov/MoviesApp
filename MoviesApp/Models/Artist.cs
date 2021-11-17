using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MoviesApp.Models
{
    public class Artist
    {
        public Artist()
        {
            
        }
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        
        [DataType(DataType.Date)] 
        public DateTime BirthdayDate { get; set; }

        public virtual ICollection<ArtistsMovie> ArtistsMovies { get; set; }
    }
}