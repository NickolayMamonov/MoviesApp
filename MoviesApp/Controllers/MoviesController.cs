using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoviesApp.Data;
using MoviesApp.Models;
using MoviesApp.Services;
using MoviesApp.Services.Dto;
using MoviesApp.ViewModels;


namespace MoviesApp.Controllers
{
    public class MoviesController: Controller
    {
        private readonly MoviesContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly IMapper _mapper;
        private readonly IMovieService _service;


        public MoviesController(MoviesContext context, ILogger<HomeController> logger, IMapper mapper, IMovieService service)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _service = service;
        }

        // GET: Movies
        [HttpGet]
        [Authorize]
        public IActionResult Index()
        {
            var movies = _mapper.Map<IEnumerable<MovieDto>, IEnumerable<MovieViewModel>>(_service.GetAllMovies().ToList());
            return View(movies);
        }
        
        // GET: Movies/Details/5
        [HttpGet]
        [Authorize]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viewModel = _mapper.Map<MovieViewModel>(_service.GetMovie((int) id));
            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }
        
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            InputMovieViewModel movie = new InputMovieViewModel();
            PopulateAssignedMovieData(movie);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create([Bind("Title,ReleaseDate,Genre,Price")] 
            InputMovieViewModel inputModel, string[] selOptions)
        {
            
            if (ModelState.IsValid)
            {
                MovieDto newMovie = _mapper.Map<MovieDto>(inputModel);
                newMovie.SelectOptions = selOptions;
                _service.AddMovie(newMovie);
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
        [Authorize(Roles = "Admin")] 
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            
            EditMovieViewModel editModel = _mapper.Map<EditMovieViewModel>(_service.GetMovie((int) id));
            
            if (editModel == null)
            {
                return NotFound();
            }
            PopulateAssignedMovieData(editModel);
            return View(editModel);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")] 
        public IActionResult Edit(int id, [Bind("Title,ReleaseDate,Genre,Price")] 
            EditMovieViewModel editModel, string[] selOptions)
        {
            var movieUp = _mapper.Map<EditMovieViewModel>(editModel);
            if (ModelState.IsValid)
            {
                try
                {
                    var movieDto = _mapper.Map<MovieDto>(editModel);
                    movieDto.SelectOptions = selOptions.ToList();
                    movieDto.Id = id;
                    movieUp = _mapper.Map<EditMovieViewModel>(_service.UpdateMovie(movieDto));
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
        [Authorize(Roles = "Admin")] 
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            DeleteMovieViewModel deleteModel = _mapper.Map<MovieDto, DeleteMovieViewModel>(_service.GetMovie((int) id));

            
            if (deleteModel == null)
            {
                return NotFound();
            }

            return View(deleteModel);
        }
        
        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")] 
        public IActionResult DeleteConfirmed(int id)
        {
            _logger.LogInformation($"Movie with id {id} has been deleted!");
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
        
            
    }
    
}