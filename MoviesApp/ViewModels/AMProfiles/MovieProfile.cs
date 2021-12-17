using AutoMapper;
using MoviesApp.Services.Dto;
using MoviesApp.ViewModels;

namespace MoviesApp.AMProfiles
{
    public class MovieProfile : Profile
    {
        public MovieProfile()
        {
            CreateMap<MovieDto, InputMovieViewModel>().ReverseMap();
            CreateMap<MovieDto, DeleteMovieViewModel>();
            CreateMap<MovieDto, EditMovieViewModel>().ReverseMap();
            CreateMap<MovieDto, MovieViewModel>().ReverseMap();
        }
    }
}