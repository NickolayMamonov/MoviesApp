using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoviesApp.Data;
using MoviesApp.Models;
using MoviesApp.ViewModels;

namespace MoviesApp.Controllers
{
    public class MoviesController: Controller
    {
        private readonly MoviesContext _context;
        private readonly ILogger<HomeController> _logger;


        public MoviesController(MoviesContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Movies
        [HttpGet]
        public IActionResult Index()
        {
            return View(_context.Movies.Select(m => new MovieViewModel
            {
                Id = m.Id,
                Genre = m.Genre,
                Price = m.Price,
                Title = m.Title,
                ReleaseDate = m.ReleaseDate
            }).ToList());
        }
        
        // GET: Movies/Details/5
        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viewModel = _context.Movies.Where(m => m.Id == id).Select(m => new MovieViewModel
            {
                Id = m.Id,
                Genre = m.Genre,
                Price = m.Price,
                Title = m.Title,
                ReleaseDate = m.ReleaseDate,
                Artists = (ICollection<Artist>) m.ArtistsMovies.Where(ma => ma.MovieId == id)
                    .Select(m => m.Artist),
                ArtistsMovies = (ICollection<ArtistsMovie>) m.ArtistsMovies.Where(ma => ma.MovieId == id)
                    .Select(m => m)
            }).FirstOrDefault();
            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }
        
        [HttpGet]
        [ValidateAntiForgeryToken]
        public IActionResult Create()
        {
            InputMovieViewModel movie = new InputMovieViewModel();
            PopulateAssignedMovieData(movie);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Title,ReleaseDate,Genre,Price")] 
            InputMovieViewModel inputModel, string[] selOptions)
        {
            var newMovie = new Movie
            {
                Title = inputModel.Title,
                Genre = inputModel.Genre,
                ReleaseDate = inputModel.ReleaseDate,
                Price = inputModel.Price,
                ArtistsMovies= inputModel.ArtistsMovies
            };
            if (ModelState.IsValid)
            {
                _context.Add(newMovie);
                _context.SaveChanges();
                if (selOptions != null)
                {
                    foreach (var artist in selOptions)
                    {
                        var artistToAdd = new ArtistsMovie
                        {
                            ArtistId = int.Parse(artist),
                            MovieId = newMovie.Id
                        };
                        _context.ArtistsMovies.Add(artistToAdd);
                        _context.SaveChanges();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            PopulateAssignedMovieData(inputModel);
            return View(inputModel);
        }
        private void PopulateAssignedMovieData(InputMovieViewModel movie)
        {
            var allOptions = _context.Artists;
            var currentOptionIDs = new HashSet<int>(movie.ArtistsMovies.Select(m => m.ArtistId));
            var checkBoxes = new List<OptionsMovies>();
            foreach (var option in allOptions)
            {
                checkBoxes.Add(new OptionsMovies
                {
                    Id = option.Id,
                    Name = option.Firstname + " " + option.Lastname,
                    Assigned = currentOptionIDs.Contains(option.Id)
                });
            }
            
            ViewData["ArtistOptions"] = checkBoxes;
        }
        
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            
            var editModel = _context.Movies.Where(m => m.Id == id).AsNoTracking()
                .Select(m => new EditMovieViewModel
                {
                    Title = m.Title,
                    Genre = m.Genre,
                    ReleaseDate = m.ReleaseDate,
                    Price = m.Price,
                    ArtistsMovies = m.ArtistsMovies
                }).FirstOrDefault();
            
            if (editModel == null)
            {
                return NotFound();
            }
            PopulateAssignedMovieData(editModel);
            return View(editModel);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Title,ReleaseDate,Genre,Price")] 
            EditMovieViewModel editModel, string[] selOptions)
        {
            var movieToUpdate = _context.Movies
                .Include(m => m.ArtistsMovies)
                .ThenInclude(am => am.Artist)
                .SingleOrDefault(m => m.Id == id);
            if (ModelState.IsValid)
            {
                try
                {
                    UpdMoviesArtists(selOptions, movieToUpdate);
                    if (movieToUpdate != null)
                    {
                        movieToUpdate.Title = editModel.Title;
                        movieToUpdate.Genre = editModel.Genre;
                        movieToUpdate.ReleaseDate = editModel.ReleaseDate;
                        movieToUpdate.Price = editModel.Price;
                        _context.Update(movieToUpdate);
                    }
                    _context.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    if (!MovieExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            PopulateAssignedMovieData(editModel);
            return View(editModel);
        }
        
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deleteModel = _context.Movies.Where(m => m.Id == id).Select(m => new DeleteMovieViewModel
            {
                Genre = m.Genre,
                Price = m.Price,
                Title = m.Title,
                ReleaseDate = m.ReleaseDate
            }).FirstOrDefault();
            
            if (deleteModel == null)
            {
                return NotFound();
            }

            return View(deleteModel);
        }
        
        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var movie = _context.Movies.Find(id);
            var сommunications = _context.ArtistsMovies.Where(ma => ma.MovieId == id)
                .Select(ma => ma).ToList();
            foreach (var elem in сommunications)
            {
                _context.ArtistsMovies.Remove(elem);
            }
            _context.Movies.Remove(movie);
            _context.SaveChanges();
            _logger.LogError($"Movie with id {movie.Id} has been deleted!");
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
        private void UpdMoviesArtists(string[] selOptions, Movie movieToUpdate)
        {
            if (selOptions == null)
            {
                movieToUpdate.ArtistsMovies = new List<ArtistsMovie>();
                return;
            }

            var selectedOptionsHS = new HashSet<string>(selOptions);
            var movieOptionsHS = new HashSet<int>(movieToUpdate.ArtistsMovies
                .Select(m => m.ArtistId));
            foreach (var option in _context.Artists)
            {
                if (selectedOptionsHS.Contains(option.Id.ToString())) // чекбокс выделен
                {
                    if (!movieOptionsHS.Contains(option.Id)) // но не отображено в таблице многие-ко-многим
                    {
                        movieToUpdate.ArtistsMovies.Add(new ArtistsMovie
                        {
                            MovieId = movieToUpdate.Id,
                            ArtistId = option.Id
                        });
                    }
                }
                else
                {
                    // чекбокс не выделен
                    if (movieOptionsHS.Contains(option.Id)) // но в таблице многие-ко-многим такое отношение было
                    {
                        ArtistsMovie movieToRemove = movieToUpdate.ArtistsMovies
                            .SingleOrDefault(m => m.ArtistId == option.Id);
                        _context.ArtistsMovies.Remove(movieToRemove ?? throw new InvalidOperationException());
                    }
                }
            }
        }
            
    }
    
}