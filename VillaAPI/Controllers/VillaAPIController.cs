using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VillaAPI.Data;
using VillaAPI.Models;
using VillaAPI.Models.DTO;
using VillaAPI.Repository.IRepository;

namespace VillaAPI.Controllers;

[ApiController] //built in support for data annotations
public class VillaApiController : ControllerBase
{
    private readonly IVillaRepository _dbVilla;
    private readonly IMapper          _mapper;
    private readonly ILogger          _logger;


    public VillaApiController(ILogger<VillaApiController> logger, IVillaRepository dbVilla, IMapper mapper)
    {
        _logger  = logger;
        _dbVilla = dbVilla;
        _mapper  = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<VillaDto>>> GetVillas()
    {
        _logger.LogInformation("Getting All villas");
        IEnumerable<Villa> villaList = await _dbVilla.GetAllAsync();
        return Ok(_mapper.Map<List<VillaDto>>(villaList));
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

        var villa = await _dbVilla.GetAsync(u => u.Id == id, true);
        if (villa == null) return NotFound();
        return Ok(_mapper.Map<VillaDto>(villa));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VillaDto>> CreateVilla([FromBody] VillaCreateDto createDto)
    {
        if (await _dbVilla.GetAsync(u => u.Name.ToLower() == createDto.Name.ToLower()) != null)
        {
            ModelState.AddModelError(nameof(createDto.Name),
                "Villa name already exists");
            {
                return BadRequest(ModelState);
            }
        }

        if (createDto == null) return BadRequest(createDto);

        Villa model = _mapper.Map<Villa>(createDto);

        await _dbVilla.CreateAsync(model);

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
        var villa = await _dbVilla.GetAsync(u => u.Id == id);
        if (villa == null) return NotFound();
        await _dbVilla.RemoveAsync(villa);
        return NoContent();
    }

    [HttpPut("{id:int}", Name = "UpdateVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    //TODO: Fix update time so that it gets updated on every update not just the first one
    public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDto updateDto)
    {
        if (updateDto == null || id != updateDto.Id) return BadRequest(updateDto);

        var villa = await _dbVilla.GetAsync(u => u.Id == id, false);
        updateDto.CreatedDate = villa.CreatedDate;

        Villa model = _mapper.Map<Villa>(updateDto);
        await _dbVilla.UpdateAsync(model);
        return NoContent();
    }

    [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
    {
        if (patchDto == null || id == 0) return BadRequest();

        var villa = await _dbVilla.GetAsync(u => u.Id == id, false);
        VillaUpdateDto villaDto = _mapper.Map<VillaUpdateDto>(villa);

        if (villa == null) return BadRequest();

        patchDto.ApplyTo(villaDto, ModelState);
        Villa model = _mapper.Map<Villa>(villaDto);
        await _dbVilla.UpdateAsync(model);

        if (!ModelState.IsValid) return BadRequest(ModelState);
        return NoContent();
    }
}