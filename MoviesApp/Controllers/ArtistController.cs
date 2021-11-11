using System;
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
                BirthdayDate = m.BirthdayDate
            }).FirstOrDefault();
            var viewMovie = _context.ArtistsMovies.Where(v => v.ArtistId == id).Select(v => new ArtistsMoviesViewModel
            {
                ArtistId = v.ArtistId,
                MovieId = v.MovieId
            });


            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }
        
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Firstname,Lastname,BirthdayDate")] InputArtistsViewModel inputModel)
        {
            if (ModelState.IsValid)
            {
                _context.Artists.Add(new Artist
                {
                    Firstname = inputModel.Firstname,
                    Lastname = inputModel.Lastname,
                    BirthdayDate = inputModel.BirthdayDate
                });
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            return View(inputModel);
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
                BirthdayDate = m.BirthdayDate
            }).FirstOrDefault();
            
            if (editModel == null)
            {
                return NotFound();
            }
            
            return View(editModel);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Firstname,Lastname,BirthdayDate")] EditArtistsViewModel editModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var artist = new Artist
                    {
                        Id = id,
                        Firstname = editModel.Firstname,
                        Lastname = editModel.Lastname,
                        BirthdayDate = editModel.BirthdayDate
                    };
                    
                    _context.Update(artist);
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
            _context.Artists.Remove(artist);
            _context.SaveChanges();
            _logger.LogError($"Artist with id {artist.Id} has been deleted!");
            return RedirectToAction(nameof(Index));
        }

        private bool ArtistExists(int id)
        {
            return _context.Artists.Any(e => e.Id == id);
        }
    }
}