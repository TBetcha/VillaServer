using VillaAPI.Models.DTO;

namespace VillaAPI.Data;

public class VillaStore
{
    public static List<VillaDTO> villaList = new()
    {
        new VillaDTO { Id = 1, Name = "Pool View", Occupancy = 100, SqFt = 5 },
        new VillaDTO { Id = 2, Name = "Beach View", Occupancy = 200, SqFt = 10 }
    };
}