using System.Linq;
using AutoMapper;
using MoviesApp.Models;

namespace MoviesApp.Services.Dto
{
    public class MovieDtoProfile : Profile
    {
        public MovieDtoProfile()
        {
            CreateMap<Movie, MovieDto>().ForMember(e => e.ArtistsMovies,
                    opt => opt.MapFrom(m => m.ArtistsMovies))
                .ReverseMap()
                .ForMember(e => e.ArtistsMovies,
                    opt => opt.MapFrom(m => m.ArtistsMovies));
            CreateMap<Movie, MovieDtoApi>().ForMember(e => e.ArtistsMoviesIds,
                    opt =>
                        opt.MapFrom(m => m.ArtistsMovies.Select(a => a.ArtistId)))
                .ReverseMap()
                .ForMember(e => e.ArtistsMovies,
                    opt => opt.Ignore());
            CreateMap<MovieDto, MovieDtoApi>().ForMember(e => e.ArtistsMoviesIds,
                arg => arg.MapFrom(
                    opt => opt.SelectOptions.Select(int.Parse)));
        }
    }
}