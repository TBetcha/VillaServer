using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VillaAPI.Data;
using VillaAPI.Models;
using VillaAPI.Models.DTO;

namespace VillaAPI.Controllers;

//[Route("api/[controller]")]
[Route("api/villaAPI")]
[ApiController] //built in support for data annotations
public class VillaAPIController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger _logger;


    public VillaAPIController(ILogger<VillaAPIController> logger, ApplicationDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    [HttpGet]
    public ActionResult<IEnumerable<VillaDto>> GetVillas()
    {
        _logger.LogInformation("Getting All villas");
        return Ok(_db.Villas.ToList());
    }

    [HttpGet("{id:int}", Name = "GetVillaById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<VillaDto> GetVilla(int id)
    {
        if (id == 0)
        {
            _logger.LogError($"Get Villa error with id {id}");
            return BadRequest();
        }

        var villa = _db.Villas.FirstOrDefault(u => u.Id == id);
        if (villa == null) return NotFound();
        return Ok(villa);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<VillaDto> CreateVilla([FromBody] VillaCreateDto villaDto)
    {
        //* data annotation - this happen automatically if api controller is used
        // if (!ModelState.IsValid)
        // {
        //     return BadRequest(ModelState);
        // }
        if (_db.Villas.FirstOrDefault(u =>
                u.Name.ToLower() == villaDto.Name.ToLower()) != null)
        {
            ModelState.AddModelError(nameof(villaDto.Name),
                "Villa name already exists");
            {
                return BadRequest(ModelState);
            }
        }

        if (villaDto == null) return BadRequest(villaDto);
        //* Removed this when villcreatedto was usec
        // if (villaDto.Id > 0)
        //     return StatusCode(StatusCodes.Status500InternalServerError);
        // {
        //
        //     Response.StatusCode = StatusCodes.Status500InternalServerError;
        //     return new JsonResult("Bad request bro");
        // }
        Villa model = new()
        {
            Amenity = villaDto.Amenity,
            Details = villaDto.Details,
            ImageUrl = villaDto.ImageUrl,
            Name = villaDto.Name,
            Occupancy = villaDto.Occupancy,
            Rate = villaDto.Rate,
            Sqft = villaDto.Sqft,
            CreatedDate =   DateTime.UtcNow
        };
        _db.Villas.Add(model);
        _db.SaveChanges();

        // for created at route I need to reference name, meaning it has to be in controller
        return CreatedAtRoute("GetVillaById", new { id = model.Id }, model);
    }

    [HttpDelete("{id:int}", Name = "DeleteVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult DeleteVilla(int id)
    {
        if (id == 0) return BadRequest();
        var villa = _db.Villas.FirstOrDefault(u => u.Id == id);
        if (villa == null) return NotFound();
        _db.Villas.Remove(villa);
        _db.SaveChanges();
        return NoContent();
    }

    [HttpPut("{id:int}", Name = "UpdateVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult UpdateVilla(int id, [FromBody] VillaUpdateDto villaDto)
    {
        if (villaDto == null || id != villaDto.Id) return BadRequest(villaDto);


        Villa model = new()
        {
            Amenity = villaDto.Amenity,
            Details = villaDto.Details,
            Id = villaDto.Id,
            ImageUrl = villaDto.ImageUrl,
            Name = villaDto.Name,
            Occupancy = villaDto.Occupancy,
            Rate = villaDto.Rate,
            Sqft = villaDto.Sqft,
            UpdatedDate =   DateTime.UtcNow
        };
        _db.Villas.Update(model);
        _db.SaveChanges();
        return NoContent();
    }

    [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
    {
        if (patchDto == null || id == 0) return BadRequest();

        var villa = _db.Villas.AsNoTracking().FirstOrDefault(u => u.Id == id);
        VillaUpdateDto villaDto = new()
        {
            Amenity = villa.Amenity,
            Details = villa.Details,
            Id = villa.Id,
            ImageUrl = villa.ImageUrl,
            Name = villa.Name,
            Occupancy = villa.Occupancy,
            Rate = villa.Rate,
            Sqft = villa.Sqft,
        };
        if (villa == null) return BadRequest();

        patchDto.ApplyTo(villaDto, ModelState);
        var model = new Villa
        {
            Amenity = villaDto.Amenity,
            Details = villaDto.Details,
            Id = villaDto.Id,
            ImageUrl = villaDto.ImageUrl,
            Name = villaDto.Name,
            Occupancy = villaDto.Occupancy,
            Rate = villaDto.Rate,
            Sqft = villaDto.Sqft,
            CreatedDate =   villa.CreatedDate,
            UpdatedDate =   DateTime.UtcNow
        };
        _db.Villas.Update(model);
        _db.SaveChanges();

        if (!ModelState.IsValid) return BadRequest(ModelState);

        return NoContent();
    }
}