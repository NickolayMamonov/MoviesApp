using System.Collections.Generic;
using MoviesApp.Services.Dto;
using static MoviesApp.Services.Dto.ArtistDto;

namespace MoviesApp.Services
{
    public interface IArtistService
    {
        ArtistDto GetArtist(int id);
        IEnumerable<ArtistDto> GetAllArtists();
        ArtistDto UpdateArtist(ArtistDto artistDto);
        ArtistDto AddArtist(ArtistDto artistDto);
        ArtistDto DeleteArtist(int id);

        IEnumerable<ArtistDtoAPI> GetAllArtistAPI();
        ArtistDtoAPI GetArtistAPI(int id);
        ArtistDtoAPI AddArtistAPI(ArtistDtoAPI inputDtoAPI);
        ArtistDtoAPI UpdateArtistAPI(ArtistDtoAPI artistDto);

       
    }
}