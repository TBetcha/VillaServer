using Microsoft.AspNetCore.Mvc;
using VillaAPI.Data;
using VillaAPI.Models.DTO;

namespace VillaAPI.Controllers;

[Route("api/villaAPI")]
[ApiController]
public class VillaAPIController : ControllerBase
{
    [HttpGet]
    public IEnumerable<VillaDTO> GetVillas()
    {
        return VillaStore.villaList;
    }

    [HttpGet("id")]
    public VillaDTO GetVilla(int id)
    {
        return VillaStore.villaList.FirstOrDefault(u => u.Id == id);
    }
}