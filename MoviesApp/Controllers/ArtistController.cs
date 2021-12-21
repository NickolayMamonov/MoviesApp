using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoviesApp.Data;
using MoviesApp.Filters;
using MoviesApp.Models;
using MoviesApp.Services;
using MoviesApp.Services.Dto;
using MoviesApp.ViewModels;

namespace MoviesApp.Controllers
{
    public class ArtistsController: Controller
    {
        private readonly MoviesContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly IMapper _mapper;
        private readonly IArtistService _service;


        public ArtistsController(MoviesContext context, ILogger<HomeController> logger, IMapper mapper, IArtistService service)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _service = service;
        }
        
        [HttpGet]
        [Authorize]
        public IActionResult Index()
        {
            var artists = _mapper.Map<IEnumerable<ArtistDto>, IEnumerable<ArtistViewModel>>(_service.GetAllArtists().ToList());
            return View(artists);
        }

        
        [HttpGet]
        [Authorize]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viewModel = _mapper.Map<ArtistViewModel>(_service.GetArtist((int) id));

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
            var artist = new InputArtistsViewModel();
            PopulateAssignedMovieData(artist);
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckAgeArtists]
        [Authorize(Roles = "Admin")]
        public IActionResult Create([Bind("Firstname,Lastname,BirthdayDate")] InputArtistsViewModel inputModel,
            string[] selOptions)
        {
            
            if (ModelState.IsValid)
            {
                ArtistDto newArtist = _mapper.Map<ArtistDto>(inputModel);
                newArtist.SelectOptions = selOptions;
                _service.AddArtist(newArtist);
                return RedirectToAction(nameof(Index));
            }

            PopulateAssignedMovieData(inputModel);
            return View(inputModel);
        }
        
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            EditArtistsViewModel editModel = _mapper.Map<EditArtistsViewModel>(_service.GetArtist((int) id));
            
            if (editModel == null)
            {
                return NotFound();
            }
            PopulateAssignedMovieData(editModel);
            return View(editModel);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckAgeArtists]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id, [Bind("Firstname,Lastname,BirthdayDate")] EditArtistsViewModel editModel,string[] selOptions)
        {
            var ArtistUP = _mapper.Map<EditArtistsViewModel>(editModel);
            if (ModelState.IsValid)
            {
                try
                {
                    var artistDto = _mapper.Map<ArtistDto>(editModel);
                    artistDto.SelectOptions = selOptions.ToList();
                    artistDto.Id = id;
                    ArtistUP = _mapper.Map<EditArtistsViewModel>(_service.UpdateArtist(artistDto));
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
            PopulateAssignedMovieData(ArtistUP);
            return View(ArtistUP);
        }
        
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            DeleteArtistsViewModel deleteModel =
                _mapper.Map<ArtistDto, DeleteArtistsViewModel>(_service.GetArtist((int) id));

            
            if (deleteModel == null)
            {
                return NotFound();
            }

            return View(deleteModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirmed(int id)
        {
            var artist = _mapper.Map<ArtistDto>(_service.DeleteArtist(id));
            _logger.LogInformation($"Artist with id {id} has been deleted!");
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

        private bool ArtistExists(int id)
        {
            return _context.Artists.Any(e => e.Id == id);
        }
    }
}