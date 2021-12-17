using System;
using System.Collections.Generic;
using MoviesApp.Models;

namespace MoviesApp.ViewModels
{
    public class ArtistViewModel:InputArtistsViewModel
    {
        public ArtistViewModel() : base()
        {
        }
        public int Id { get; set; }
    }
}   