using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/VillaAPI")]
    //mevcut apiyi tüketen bütün kullanıcılar, controller ismi değişmesi halinde değişikliğe gitmek zorunda.
    //Bu sebeple hard coded kullanmak daha avantajlı 
    //[Route("api/[controller]")]//in this way it takes the current controller itself
    [ApiController]

    public class VillaAPIController : ControllerBase
    {
        //private readonly ILogger<VillaAPIController> _logger;
        //public VillaAPIController(ILogger<VillaAPIController> logger)
        //{
        //    _logger = logger;
        //}

        [HttpGet]
        //public IEnumerable<VillaDTO> GetVillas()
        //ActionResult ile statuscode değerleri dönmeye başlamıştır.
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            //_logger.LogInformation("gettin all villas");
            return Ok(VillaStore.villaList);
            //************************************
            //liste static olarak bir class içinden çağrılmaya başlandı
            //return VillaStore.villaList;
            //************************************
            //return new List<VillaDTO>
            //{
            //    new VillaDTO{Id=1,Name="Pool View"},
            //    new VillaDTO{Id=2,Name="Beach View"}
            //};
        }

        //eğer http tanımlaması yapılmazsa default olarak get alınır
        //[HttpGet]
        //[HttpGet("id")]
        [HttpGet("{id:int}", Name = "GetVilla")]
        //producesresponsetype ile durum kodlarının altında yer alan undocumented ifadesi ortadan kaldırılmıştır
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VillaDTO))]
        //[ProducesResponseType(200, Type = typeof(VillaDTO))]
        //actionresult yanındaki villadto kaldırılarak yukarıya eklendi
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        //burada id değeri belirtilmez ve sadece httpget yazılırsa exception fırlatır.
        public ActionResult<VillaDTO> GetVilla(int id)
        {
            if (id == 0)
            {
                //_logger.LogError("get villa error with id "+id);
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(x => x.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            return Ok(villa);
            //FirstOrDefault kullanıldığı için eğer geçersiz bir id gelirse null dönecektir
        }

        [HttpPost]
        //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VillaDTO))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //daha anlaşılır hata kodları vermek için productresponsetype kullanılır
        public ActionResult<VillaDTO> CreateVilla([FromBody] VillaDTO villaDTO)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            if (VillaStore.villaList.FirstOrDefault(u => u.Name.ToLower() == villaDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Villa already exist!");
                return BadRequest(ModelState);
            }
            if (villaDTO == null)
            {
                return BadRequest(villaDTO);
            }
            if (villaDTO.Id == 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            villaDTO.Id = VillaStore.villaList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
            VillaStore.villaList.Add(villaDTO);

            //return Ok(villaDTO);
            return CreatedAtRoute("GetVilla", new { id = villaDTO.Id }, villaDTO);
            //deneme
        }

        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            VillaStore.villaList.Remove(villa);
            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        public IActionResult UpdateVilla(int id, [FromBody] VillaDTO villaDTO)
        {
            if (villaDTO == null || id != villaDTO.Id)
            {
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            villa.Name = villaDTO.Name;
            villa.Sqft = villaDTO.Sqft;
            villa.Occupancy = villaDTO.Occupancy;

            return NoContent();
        }

        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            if (villa == null)
            {
                return BadRequest();
            }
            patchDTO.ApplyTo(villa, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }

    }
}
