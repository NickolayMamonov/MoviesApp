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
    public class ArtistsController: Controller
    {
        private readonly MoviesContext _context;
        private readonly ILogger<HomeController> _logger;


        public ArtistsController(MoviesContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        [HttpGet]
        public IActionResult Index()
        {
            return View(_context.Artists.Select(m => new ArtistViewModel
            {
                Id = m.Id,
                Firstname = m.Firstname,
                Lastname = m.Lastname,
                BirthdayDate = m.BirthdayDate
            }).ToList());
        }

        
        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viewModel = _context.Artists.Where(m => m.Id == id).Select(m => new ArtistViewModel
            {
                Id = m.Id,
                Firstname = m.Firstname,
                Lastname = m.Lastname,
                BirthdayDate = m.BirthdayDate,
                Movies = (ICollection<Movie>) m.ArtistsMovies.Where(we => we.ArtistId == id).Select(m => m.Movie),
               ArtistsMovies = (ICollection<ArtistsMovie>) m.ArtistsMovies.Where(we => we.ArtistId == id).Select(m => m) 
            }).FirstOrDefault();
            


            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }
        
        [HttpGet]
        public IActionResult Create()
        {
            var artist = new InputArtistsViewModel();
            PopulateAssignedMovieData(artist);
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public IActionResult Create([Bind("Firstname,Lastname,BirthdayDate")] InputArtistsViewModel artist,
            string[] selOptions)
        {
            var newArtist = new Artist
            {
                Firstname = artist.Firstname,
                Lastname = artist.Lastname,
                BirthdayDate = artist.BirthdayDate,
                ArtistsMovies = artist.ArtistsMovies
            };
            if (ModelState.IsValid)
            {
                _context.Artists.Add(newArtist);
                _context.SaveChanges();
                if (selOptions != null)
                {
                    foreach (var movie in selOptions)
                    {
                        var movieToAdd = new ArtistsMovie
                        {
                            ArtistId = newArtist.Id,
                            MovieId = int.Parse(movie)
                        };
                        _context.ArtistsMovies.Add(movieToAdd);
                        _context.SaveChanges();
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            PopulateAssignedMovieData(artist);
            return View(artist);
        }
        
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var editModel = _context.Artists.Where(m => m.Id == id).Select(m => new EditArtistsViewModel
            {
                Firstname = m.Firstname,
                Lastname = m.Lastname,
                BirthdayDate = m.BirthdayDate,
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
        public IActionResult Edit(int id, [Bind("Firstname,Lastname,BirthdayDate")] EditArtistsViewModel editModel,string[] selOptions)
        {
            var ArtistUP = _context.Artists
                .Include(a => a.ArtistsMovies)
                .ThenInclude(am => am.Movie)
                .SingleOrDefault(a => a.Id == id);
            if (ModelState.IsValid)
            {
                try
                {
                    UpdArtistsMovies(selOptions, ArtistUP);
                    if (ArtistUP != null)
                    {
                        ArtistUP.Firstname = editModel.Firstname;
                        ArtistUP.Lastname = editModel.Lastname;
                        ArtistUP.BirthdayDate = editModel.BirthdayDate;
                        _context.Update(ArtistUP);
                    }

                    _context.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    if (!ArtistExists(id))
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

            var deleteModel = _context.Artists.Where(m => m.Id == id).Select(m => new DeleteArtistsViewModel
            {
                Firstname = m.Firstname,
                Lastname = m.Lastname, 
                BirthdayDate= m.BirthdayDate
            }).FirstOrDefault();
            
            if (deleteModel == null)
            {
                return NotFound();
            }

            return View(deleteModel);
        }
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var artist = _context.Artists.Find(id);
            var сom = _context.ArtistsMovies.Where(ma => ma.ArtistId == id)
                .Select(ma => ma).ToList();
            foreach (var elem in сom)
            {
                _context.ArtistsMovies.Remove(elem);
            }
            _context.Artists.Remove(artist);
            _context.SaveChanges();
            _logger.LogError($"Artist with id {artist.Id} has been deleted!");
            return RedirectToAction(nameof(Index));
        }
        private void PopulateAssignedMovieData(InputArtistsViewModel artist)
        {
            var allOptions = _context.Movies;
            var currentOptionIDs = new HashSet<int>(artist.ArtistsMovies.Select(m => m.MovieId));
            var checkBoxes = new List<OptionsArtists>();
            foreach (var option in allOptions)
            {
                checkBoxes.Add(new OptionsArtists
                {
                    Id = option.Id,
                    Name = option.Title,
                    Assigned = currentOptionIDs.Contains(option.Id)
                });
            }

            ViewData["MovieOptions"] = checkBoxes;
        }
        private void UpdArtistsMovies(string[] selOptions, Artist artistToUpdate)
        {
            if (selOptions == null)
            {
                artistToUpdate.ArtistsMovies = new List<ArtistsMovie>();
                return;
            }

            var selOptionsHS = new HashSet<string>(selOptions);
            var artistOptionsHS = new HashSet<int>(artistToUpdate.ArtistsMovies
                .Select(m => m.MovieId));
            foreach (var option in _context.Movies)
            {
                if (selOptionsHS.Contains(option.Id.ToString())) // чекбокс выделен
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

        private bool ArtistExists(int id)
        {
            return _context.Artists.Any(e => e.Id == id);
        }
    }
}