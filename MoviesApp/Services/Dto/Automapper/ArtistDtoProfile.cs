using System.Linq;
using AutoMapper;
using MoviesApp.Models;

namespace MoviesApp.Services.Dto
{
    public class ArtistDtoProfile : Profile
    {
        public ArtistDtoProfile()
        {
            CreateMap<Artist, ArtistDto>().ForMember(e => e.ArtistsMovies,
                    opt => opt.MapFrom(m => m.ArtistsMovies))
                .ReverseMap()
                .ForMember(e => e.ArtistsMovies,
                    opt => opt.MapFrom(m => m.ArtistsMovies));
            CreateMap<Artist, ArtistDtoAPI>().ForMember(e => e.ArtistsMoviesIds,
                    opt =>
                        opt.MapFrom(m => m.ArtistsMovies.Select(a => a.MovieId)))
                .ReverseMap()
                .ForMember(e => e.ArtistsMovies,
                    opt => opt.Ignore());
            CreateMap<ArtistDto, ArtistDtoAPI>().ForMember(e => e.ArtistsMoviesIds,
                arg => arg.MapFrom(
                    opt => opt.SelectOptions.Select(int.Parse)));
        }
    }
}