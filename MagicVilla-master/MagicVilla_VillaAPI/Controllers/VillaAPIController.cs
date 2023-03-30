using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Logging;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/VillaAPI")]
    [ApiController]

    public class VillaAPIController : ControllerBase
    {
        //private readonly ApplicationDBContext _db;
        private readonly IVillaRepository _dbVilla;
        private readonly IMapper _mapper;
        //public VillaAPIController(ApplicationDBContext db, IMapper mapper)
        public VillaAPIController(IVillaRepository dbVilla, IMapper mapper)
        {
            _dbVilla = dbVilla;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        {
            //_logger.Log("getting all values","");
            //return Ok(VillaStore.villaList);
            //IEnumerable<Villa> villaList = await _dbVilla.Villas.ToListAsync();
            IEnumerable<Villa> villaList = await _dbVilla.GetAllAsync();
            //return Ok(await _db.Villas.ToListAsync());
            return Ok(_mapper.Map<List<VillaDTO>>(villaList));
        }

        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VillaDTO))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<VillaDTO>> GetVilla(int id)
        {
            if (id == 0)
            {
                //_logger.Log("get villa error with id " + id,"error");
                return BadRequest();
            }
            //var villa = VillaStore.villaList.FirstOrDefault(x => x.Id == id);
            //var villa = await _db.Villas.FirstOrDefaultAsync(x => x.Id == id);
            var villa = await _dbVilla.GetAsync(x => x.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            //return Ok(villa);
            return Ok(_mapper.Map<VillaDTO>(villa));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody] VillaCreateDTO createDTO)
        {
            //if (await _db.Villas.FirstOrDefaultAsync(u => u.Name.ToLower() == createDTO.Name.ToLower()) != null)
            if (await _dbVilla.GetAsync(u => u.Name.ToLower() == createDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Villa already exist!");
                return BadRequest(ModelState);
            }
            if (createDTO == null)
            {
                return BadRequest(createDTO);
            }
            if (createDTO.Id == 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            //villaDTO.Id = VillaStore.villaList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;

            Villa model = _mapper.Map<Villa>(createDTO);
            //Villa model = new()
            //{
            //    Amenity = createDTO.Amenity,
            //    Details = createDTO.Details,
            //    //Id = (int)villaDTO.Id,
            //    ImageUrl = createDTO.ImageUrl,
            //    Name = createDTO.Name,
            //    Occupancy = createDTO.Occupancy,
            //    Rate = createDTO.Rate,
            //    Sqft = createDTO.Sqft,
            //};

            //await _db.Villas.AddAsync(model);
            //await _db.SaveChangesAsync();
            await _dbVilla.CreateAsync(model);
            //var newVillaId = model.Id;
            return CreatedAtRoute("GetVilla", new { id = model.Id }, model);
            //return CreatedAtRoute("GetVilla", new { id = _db.Villas.Find(x=>x.Name==model.Name).Id }, villaDTO);
            //deneme
        }

        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            //var villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
            var villa = await _dbVilla.GetAsync(u => u.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            await _dbVilla.RemoveAsync(villa);
            //_db.Villas.Remove(villa);
            //await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDTO updateDTO)
        {
            if (updateDTO == null || id != updateDTO.Id)
            {
                return BadRequest();
            }
            //var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            //villa.Name = villaDTO.Name;
            //villa.Sqft = villaDTO.Sqft;
            //villa.Occupancy = villaDTO.Occupancy;

            Villa model = _mapper.Map<Villa>(updateDTO);
            //Villa model = new()
            //{
            //    Amenity = updateDTO.Amenity,
            //    Details = updateDTO.Details,
            //    Id = (int)updateDTO.Id,
            //    ImageUrl = updateDTO.ImageUrl,
            //    Name = updateDTO.Name,
            //    Occupancy = updateDTO.Occupancy,
            //    Rate = updateDTO.Rate,
            //    Sqft = updateDTO.Sqft,
            //};
            //_db.Villas.Update(model);
            await _dbVilla.UpdateAsync(model);
            return NoContent();
        }

        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }
            //var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            var villa = await _dbVilla.GetAsync(u => u.Id == id, tracked: false);
            VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);
            //VillaUpdateDTO villaDTO = new()
            //{
            //    Amenity = villa.Amenity,
            //    Details = villa.Details,
            //    Id = villa.Id,
            //    ImageUrl = villa.ImageUrl,
            //    Name = villa.Name,
            //    Occupancy = villa.Occupancy,
            //    Rate = villa.Rate,
            //    Sqft = villa.Sqft,
            //};

            if (villa == null)
            {
                return BadRequest();
            }
            patchDTO.ApplyTo(villaDTO, ModelState);

            Villa model = _mapper.Map<Villa>(villaDTO);
            //Villa model = new Villa()
            //{
            //    Amenity = villaDTO.Amenity,
            //    Details = villaDTO.Details,
            //    Id = villaDTO.Id,
            //    ImageUrl = villaDTO.ImageUrl,
            //    Name = villaDTO.Name,
            //    Occupancy = villaDTO.Occupancy,
            //    Rate = villaDTO.Rate,
            //    Sqft = villaDTO.Sqft,
            //};

            //_db.Villas.Update(model);
            //await _db.SaveChangesAsync();

            await _dbVilla.UpdateAsync(model);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }

    }
}
