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
public class VillaApiController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger _logger;


    public VillaApiController(ILogger<VillaApiController> logger, ApplicationDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<VillaDto>>> GetVillas()
    {
        _logger.LogInformation("Getting All villas");
        return Ok(await _db.Villas.ToListAsync());
    }

    [HttpGet("{id:int}", Name = "GetVillaById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VillaDto>> GetVilla(int id)
    {
        if (id == 0)
        {
            _logger.LogError($"Get Villa error with id {id}");
            return BadRequest();
        }

        var villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
        if (villa == null) return NotFound();
        return Ok(villa);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VillaDto>> CreateVilla([FromBody] VillaCreateDto villaDto)
    {
        //* data annotation - this happen automatically if api controller is used
        // if (!ModelState.IsValid)
        // {
        //     return BadRequest(ModelState);
        // }
        if (await _db.Villas.FirstOrDefaultAsync(u =>
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
            CreatedDate = DateTime.UtcNow
        };
        await _db.Villas.AddAsync(model);
        await _db.SaveChangesAsync();

        // for created at route I need to reference name, meaning it has to be in controller
        return CreatedAtRoute("GetVillaById", new { id = model.Id }, model);
    }

    [HttpDelete("{id:int}", Name = "DeleteVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteVilla(int id)
    {
        if (id == 0) return BadRequest();
        var villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
        if (villa == null) return NotFound();
        _db.Villas.Remove(villa);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{id:int}", Name = "UpdateVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDto villaDto)
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
            UpdatedDate = DateTime.UtcNow
        };
        _db.Villas.Update(model);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
    {
        if (patchDto == null || id == 0) return BadRequest();

        var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
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
            CreatedDate = villa.CreatedDate,
            UpdatedDate = DateTime.UtcNow
        };
        _db.Villas.Update(model);
        await _db.SaveChangesAsync();

        if (!ModelState.IsValid) return BadRequest(ModelState);

        return NoContent();
    }
}