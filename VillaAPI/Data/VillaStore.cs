using VillaAPI.Models.DTO;

namespace VillaAPI.Data;

public class VillaStore
{
    public static List<VillaDTO> villaList = new()
    {
        new() { Id = 1, Name = "Pool View" },
        new() { Id = 2, Name = "Beach View" }
    };
}