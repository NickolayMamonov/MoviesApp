using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MoviesApp.Services.Dto
{
    public class ArtistDtoAPI
    {
        public int? Id { get; set; }
        
        [Required]
        public string FirstName { get; set; }
        
        [Required]
        public string LastName { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime BirthdayDate { get; set; }

        public ICollection<int> ArtistsMoviesIds { get; set; }
    }
}