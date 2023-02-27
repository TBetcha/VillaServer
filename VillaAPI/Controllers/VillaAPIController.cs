using Microsoft.AspNetCore.Mvc;
using VillaAPI.Data;
using VillaAPI.Models.DTO;

namespace VillaAPI.Controllers;

//[Route("api/[controller]")]
[Route("api/villaAPI")]
[ApiController] //built in support for data annotations
public class VillaAPIController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<VillaDTO>> GetVillas()
    {
        return Ok(VillaStore.villaList);
    }

    [HttpGet("id:int", Name = "GetVillaById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<VillaDTO> GetVilla(int id)
    {
        if (id == 0) return BadRequest();
        var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
        if (villa == null) return NotFound();
        return Ok(villa);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<VillaDTO> CreateVilla([FromBody] VillaDTO villaDTO)
    {
        // data annotation
        // if (!ModelState.IsValid)
        // {
        //     return BadRequest(ModelState);
        // }
        if (VillaStore.villaList.FirstOrDefault(u => u.Name.ToLower() == villaDTO.Name.ToLower()) != null)
        {
            ModelState.AddModelError(nameof(villaDTO.Name), "Villa name already exists");
            {
                return BadRequest(ModelState);
            }

        }
        if (villaDTO == null) return BadRequest(villaDTO);
        if (villaDTO.Id > 0) return StatusCode(StatusCodes.Status500InternalServerError);
        // {
        //
        //     Response.StatusCode = StatusCodes.Status500InternalServerError;
        //     return new JsonResult("Bad request bro");
        // }

        villaDTO.Id = VillaStore.villaList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
        VillaStore.villaList.Add(villaDTO);

        return CreatedAtRoute("GetVillaById", new { id = villaDTO.Id }, villaDTO);
    }
}