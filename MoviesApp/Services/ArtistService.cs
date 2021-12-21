using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MoviesApp.Data;
using MoviesApp.Models;
using MoviesApp.Services.Dto;

namespace MoviesApp.Services
{
    public class ArtistService : IArtistService
    {
        private readonly MoviesContext _context;
        private readonly IMapper _mapper;

        public ArtistService(MoviesContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public ArtistDto GetArtist(int id)
        {
            return _mapper.Map<ArtistDto>(
                _context.Artists
                    .Include(e => e.ArtistsMovies)
                    .ThenInclude(e => e.Movie)
                    .FirstOrDefault(a => a.Id == id)
            );
        }

        public IEnumerable<ArtistDto> GetAllArtists()
        {
            return _mapper.Map<IEnumerable<Artist>, IEnumerable<ArtistDto>>(
                _context.Artists.Include(e => e.ArtistsMovies).ToList()
            );
        }

        public ArtistDto UpdateArtist(ArtistDto artistDto)
        {
            if (artistDto.Id == null)
            {
                return null;
            }

            try
            {
                var artistToUpdate = _context.Artists
                    .Include(e => e.ArtistsMovies)
                    .ThenInclude(e => e.Movie)
                    .FirstOrDefault(e => e.Id == artistDto.Id);
                UpdateArtistsMovies(artistDto.SelectOptions.ToArray(), artistToUpdate);
                if (artistToUpdate != null)
                {
                    artistToUpdate.Firstname = artistDto.FirstName;
                    artistToUpdate.Lastname = artistDto.LastName;
                    artistToUpdate.BirthdayDate = artistDto.BirthdayDate;
                    _context.Update(artistToUpdate);
                    _context.SaveChanges();
                }

                return _mapper.Map<ArtistDto>(artistToUpdate);
            }
            catch (DbUpdateException)
            {
                if (!ArtistExists((int) artistDto.Id))
                {
                    return null;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public ArtistDto AddArtist(ArtistDto artistDto)
        {
            var artist = _context.Artists.Add(_mapper.Map<Artist>(artistDto)).Entity;
            _context.SaveChanges();
            if (artistDto.SelectOptions != null)
            {
                foreach (var movie in artistDto.SelectOptions)
                {
                    var id = _mapper.Map<ArtistDto>(artist).Id;
                    if (id != null)
                    {
                        var movieToAdd = new ArtistsMovie
                        {
                            ArtistId = (int) id,
                            MovieId = int.Parse(movie)
                        };
                        _context.ArtistsMovies.Add(movieToAdd);
                    }
                }
            }

            _context.SaveChanges();
            return _mapper.Map<ArtistDto>(artist);
        }

        public ArtistDto DeleteArtist(int id)
        {
            var artist = _context.Artists.Find(id);
            if (artist == null)
            {
                return null;
            }

            var coommunications = _context.ArtistsMovies.Where(ma => ma.ArtistId == id)
                .Select(ma => ma).ToList();
            foreach (var elem in coommunications)
            {
                _context.ArtistsMovies.Remove(elem);
            }

            _context.Artists.Remove(artist);
            _context.SaveChanges();

            return _mapper.Map<ArtistDto>(artist);
        }

        private bool ArtistExists(int id)
        {
            return _context.Artists.Any(e => e.Id == id);
        }

        #region API

        public IEnumerable<ArtistDtoAPI> GetAllArtistAPI()
        {
            return _mapper.Map<IEnumerable<Artist>, IEnumerable<ArtistDtoAPI>>(
                _context.Artists.Include(e => e.ArtistsMovies).ToList()
            );
        }

        public ArtistDtoAPI GetArtistAPI(int id)
        {
            return _mapper.Map<ArtistDtoAPI>(
                _context.Artists
                    .Include(e => e.ArtistsMovies)
                    .FirstOrDefault(a => a.Id == id)
            );
        }

        public ArtistDtoAPI AddArtistAPI(ArtistDtoAPI inputDtoAPI)
        {
            var artist = _context.Artists.Add(_mapper.Map<Artist>(inputDtoAPI)).Entity;
            _context.SaveChanges();
            if (inputDtoAPI.ArtistsMoviesIds != null)
            {
                foreach (var movie in inputDtoAPI.ArtistsMoviesIds)
                {
                    var id = _mapper.Map<ArtistDto>(artist).Id;
                    if (id != null)
                    {
                        var movieToAdd = new ArtistsMovie
                        {
                            ArtistId = (int) id,
                            MovieId = movie
                        };
                        _context.ArtistsMovies.Add(movieToAdd);
                    }
                }
            }

            _context.SaveChanges();
            return _mapper.Map<ArtistDtoAPI>(artist);
        }

        public ArtistDtoAPI UpdateArtistAPI(ArtistDtoAPI artistDto)
        {
            if (artistDto.Id == null)
            {
                return null;
            }

            try
            {
                var artistToUpdate = _context.Artists
                    .Include(e => e.ArtistsMovies)
                    .ThenInclude(e => e.Movie)
                    .FirstOrDefault(e => e.Id == artistDto.Id);
                UpdateArtistsMovies(artistDto.ArtistsMoviesIds.Select(e=>e.ToString()).ToArray(), artistToUpdate);
                if (artistToUpdate != null)
                {
                    artistToUpdate.Firstname = artistDto.FirstName;
                    artistToUpdate.Lastname = artistDto.LastName;
                    artistToUpdate.BirthdayDate = artistDto.BirthdayDate;
                    _context.Artists.Update(artistToUpdate);
                    _context.SaveChanges();
                }

                return _mapper.Map<ArtistDtoAPI>(artistToUpdate);
            }
            catch (DbUpdateException)
            {
                if (!ArtistExists((int) artistDto.Id))
                {
                    //упрощение для примера
                    //лучше всего генерировать ошибки и обрабатывать их на уровне конроллера
                    return null;
                }
                else
                {
                    //упрощение для примера
                    //лучше всего генерировать ошибки и обрабатывать их на уровне конроллера
                    return null;
                }
            }
        }

        #endregion


        private void UpdateArtistsMovies(string[] selectedOptions, Artist artistToUpdate)
        {
            if (selectedOptions == null)
            {
                artistToUpdate.ArtistsMovies = new List<ArtistsMovie>();
                return;
            }

            var selectedOptionsHS = new HashSet<string>(selectedOptions);
            var artistOptionsHS = new HashSet<int>(artistToUpdate.ArtistsMovies
                .Select(m => m.MovieId));
            foreach (var option in _context.Movies)
            {
                if (selectedOptionsHS.Contains(option.Id.ToString())) // чекбокс выделен
                {
                    if (!artistOptionsHS.Contains(option.Id)) // но не отображено в таблице многие-ко-многим
                    {
                        artistToUpdate.ArtistsMovies.Add(new ArtistsMovie
                        {
                            ArtistId = artistToUpdate.Id,
                            MovieId = option.Id
                        });
                    }
                }
                else
                {
                    // чекбокс не выделен
                    if (artistOptionsHS.Contains(option.Id)) // но в таблице многие-ко-многим такое отношение было
                    {
                        ArtistsMovie movieToRemove = artistToUpdate.ArtistsMovies
                            .SingleOrDefault(m => m.MovieId == option.Id);
                        _context.ArtistsMovies.Remove(movieToRemove ?? throw new InvalidOperationException());
                    }
                }
            }
        }
    }
}