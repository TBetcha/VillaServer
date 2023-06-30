using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VillaAPI.Data;
using VillaAPI.Models;
using VillaAPI.Models.DTO;
using VillaAPI.Repository.IRepository;

namespace VillaAPI.Controllers;

[Route("api/villaAPI")]
[ApiController] //built in support for data annotations
public class VillaApiController : ControllerBase
{
    private readonly IVillaRepository _dbVilla;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;
    private readonly ApiResponse _response;


    public VillaApiController(ILogger<VillaApiController> logger, IVillaRepository dbVilla, IMapper mapper)
    {
        _logger   = logger;
        _dbVilla  = dbVilla;
        _mapper   = mapper;
        _response = new ApiResponse();
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse>> GetVillas()
    {
        _logger.LogInformation("Getting All villas");
        IEnumerable<Villa> villaList = await _dbVilla.GetAllAsync();
        _response.Result     = _mapper.Map<IEnumerable<VillaDto>>(villaList);
        _response.StatusCode = HttpStatusCode.OK;
        _response.IsSuccess  = true;
        return Ok(_response);
    }

    [HttpGet("{id:int}", Name = "GetVillaById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> GetVilla(int id)
    {
        if (id == 0)
        {
            _logger.LogError($"Get Villa error with id {id}",id);
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.ErrorMessages = new List<string> { "That is not a valid ID" };
            _response.IsSuccess = false;
            return BadRequest(_response);
        }

        var villa = await _dbVilla.GetAsync(u => u.Id == id, true);

        if (villa == null)
        {
            _response.ErrorMessages = new List<string> { "ID Not Found" };
            _response.StatusCode = HttpStatusCode.NotFound;
            _response.IsSuccess  = false;
            return NotFound(_response);
        }

        _response.Result     = _mapper.Map<VillaDto>(villa);
        _response.StatusCode = HttpStatusCode.OK;
        _response.IsSuccess  = true;
        return _response;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    //TODO: Fix error handling and api response
    public async Task<ActionResult<ApiResponse>> CreateVilla([FromBody] VillaCreateDto createDto)
    {
        if (await _dbVilla.GetAsync(u => u.Name.ToLower() == createDto.Name.ToLower()) != null)
        {
            //! Do I want this and ApiResponse errors
            ModelState.AddModelError(nameof(createDto.Name),
                "Villa name already exists");
            {
                return BadRequest(ModelState);
            }
        }

        if (createDto == null) return BadRequest(createDto);

        var villa = _mapper.Map<Villa>(createDto);

        await _dbVilla.CreateAsync(villa);

        _response.Result     = _mapper.Map<VillaDto>(villa);
        _response.StatusCode = HttpStatusCode.Created;
        _response.IsSuccess  = true;
        // for created at route I need to reference name, meaning it has to be in controller
        return CreatedAtRoute("GetVillaById", new { id = villa.Id }, _response);
    }

    [HttpDelete("{id:int}", Name = "DeleteVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> DeleteVilla(int id)
    {
        if (id == 0)
        {
            _logger.LogError($"Delete Villa error with id {id}", id);
            _response.StatusCode    = HttpStatusCode.BadRequest;
            _response.ErrorMessages = new List<string> { "That is not a valid ID" };
            _response.IsSuccess     = false;
            return BadRequest(_response);
        }
        var villa = await _dbVilla.GetAsync(u => u.Id == id);
        if (villa == null) return NotFound();
        await _dbVilla.RemoveAsync(villa);

        _response.StatusCode = HttpStatusCode.NoContent;
        _response.IsSuccess  = true;
        return _response;
    }

    [HttpPut("{id:int}", Name = "UpdateVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    //TODO: Fix update time so that it gets updated on every update not just the first one
    public async Task<ActionResult<ApiResponse>> UpdateVilla(int id, [FromBody] VillaUpdateDto updateDto)
    {
        if (updateDto == null || id != updateDto.Id)
        {
            _logger.LogError($"Update Villa error with id: {id} and update dto: {updateDto}", id, updateDto);
            _response.StatusCode    = HttpStatusCode.BadRequest;
            _response.ErrorMessages = new List<string> { $"Problem with update data: {updateDto}" };
            _response.IsSuccess     = false;
            return BadRequest(_response);
        }

        var villa = await _dbVilla.GetAsync(u => u.Id == id, false);
        updateDto.CreatedDate = villa.CreatedDate;

        var model = _mapper.Map<Villa>(updateDto);
        await _dbVilla.UpdateAsync(model);

        _response.StatusCode = HttpStatusCode.NoContent;
        _response.IsSuccess  = true;
        return Ok(_response);
    }

    [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
    {
        if (patchDto == null || id == 0)
        {
            _logger.LogError($"Update Villa error with id: {id} and patch dto: {patchDto}", id, patchDto);
            _response.StatusCode    = HttpStatusCode.BadRequest;
            _response.ErrorMessages = new List<string> { $"Problem with patch data: {patchDto}" };
            _response.IsSuccess     = false;
            return BadRequest(_response);
        }

        var villa = await _dbVilla.GetAsync(u => u.Id == id, false);
        var villaDto = _mapper.Map<VillaUpdateDto>(villa);

        if (villa == null)
        {
            _logger.LogError($"That ID is not found {id}", id);
            _response.StatusCode    = HttpStatusCode.BadRequest;
            _response.ErrorMessages = new List<string> { ($"That ID is not found {id}") };
            _response.IsSuccess     = false;
            return BadRequest(_response);
        }

        patchDto.ApplyTo(villaDto, ModelState);
        var model = _mapper.Map<Villa>(villaDto);
        await _dbVilla.UpdateAsync(model);

        if (!ModelState.IsValid)
        {
            _logger.LogError($"Model state not valid");
            _response.StatusCode    = HttpStatusCode.BadRequest;
            _response.ErrorMessages = new List<string> { ($"Problem updating resource") };
            _response.IsSuccess     = false;
            return BadRequest(_response);
        }
        _response.StatusCode    = HttpStatusCode.NoContent;
        _response.IsSuccess     = true;
        return NoContent();
    }
}