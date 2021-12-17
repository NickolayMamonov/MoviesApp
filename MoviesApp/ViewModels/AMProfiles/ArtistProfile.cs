using AutoMapper;
using MoviesApp.Services.Dto;
using MoviesApp.ViewModels;

namespace MoviesApp.AMProfiles
{
    public class ArtistProfile : Profile
    {
        public ArtistProfile()
        {
            CreateMap<ArtistDto, InputArtistsViewModel>().ReverseMap();
            CreateMap<ArtistDto, DeleteArtistsViewModel>();
            CreateMap<ArtistDto, EditArtistsViewModel>().ReverseMap();
            CreateMap<ArtistDto, ArtistViewModel>().ReverseMap();
        }
    }
}