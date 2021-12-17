using System.Collections.Generic;
using MoviesApp.Services.Dto;

namespace MoviesApp.Services
{
    public interface IMovieService
    {
        MovieDto GetMovie(int id);
        IEnumerable<MovieDto> GetAllMovies();
        MovieDto UpdateMovie(MovieDto movieDto);
        MovieDto AddMovie(MovieDto movieDto);
        MovieDto DeleteMovie(int id);
        IEnumerable<MovieDtoApi> GetAllMoviesApi();

        MovieDtoApi GetMovieApi(int id);

        MovieDtoApi AddMovieApi(MovieDtoApi inputDtoApi);
        
        MovieDtoApi UpdateMovieApi(MovieDtoApi movieDto);
    }
}