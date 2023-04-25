using System.ComponentModel.DataAnnotations;

namespace VillaAPI.Models.DTO;

public class VillaCreateDto
{
    [Required]
    [MaxLength(30)]
    public string Name { get; set; }

    public string Details { get; set; }

    [Required]
    public double Rate { get; set; }

    public int Occupancy { get; set; }

    public string ImageUrl { get; set; }
    public string Amenity { get; set; }
    public int Sqft { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}