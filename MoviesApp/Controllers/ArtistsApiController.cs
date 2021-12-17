using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MoviesApp.Services;
using MoviesApp.Services.Dto;

namespace MoviesApp.Controllers
{
    [Route("API/artists")]
    [ApiController]
    public class ArtistsApiController : Controller
    {
        private readonly IArtistService _service;
        private readonly IMapper _mapper;

        public ArtistsApiController(IArtistService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }
        
        [HttpGet] // GET: /API/artists
        [ProducesResponseType(200, Type = typeof(IEnumerable<ArtistDtoAPI>))]  
        [ProducesResponseType(404)]
        public ActionResult<IEnumerable<ArtistDtoAPI>> GetArtists()
        {
            return Ok(_service.GetAllArtistAPI());
        }
        
        [HttpGet("{id}")] // GET: /API/artists/5
        [ProducesResponseType(200, Type = typeof(ArtistDtoAPI))]  
        [ProducesResponseType(404)]
        public IActionResult GetById(int id)
        {
            var artist = _service.GetArtistAPI(id);
            if (artist == null) return NotFound();  
            return Ok(artist);
        }
        
        [HttpPost] // POST: API/artists
        public ActionResult<ArtistDtoAPI> PostArtist(ArtistDtoAPI inputDtoAPI)
        {
            var artist = _service.AddArtistAPI(inputDtoAPI);
            return CreatedAtAction("GetById", new { id = artist.Id }, artist);
        }
        
        [HttpPut("{id}")] // PUT: API/artists/5
        public IActionResult UpdateArtist(int id, ArtistDtoAPI editDto)
        {
            editDto.Id = id;
            var artist = _service.UpdateArtistAPI(editDto);

            if (artist==null)
            {
                return BadRequest();
            }

            return Ok(artist);
        }
        
        [HttpDelete("{id}")] // DELETE: API/artists/5
        public ActionResult<MovieDto> DeleteArtist(int id)
        {
            var artist = _mapper.Map<ArtistDtoAPI>(_service.DeleteArtist(id));
            if (artist == null) return NotFound();  
            return Ok(artist);
        }
    }
}