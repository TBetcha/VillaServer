using Microsoft.EntityFrameworkCore;
using VillaAPI.Models;

namespace VillaAPI.Data;

public class ApplicationDbContext : DbContext
{
    //pass options on to the base class from the service in the container
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Villa> Villas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Villa>().HasData(new Villa
            {
                Id          = 1,
                Name        = "Royal Villa",
                Details     = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incidid",
                ImageUrl    = "https: //dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa3.jpg",
                Occupancy   = 5,
                Rate        = 200,
                Sqft        = 550,
                Amenity     = "",
                CreatedDate = DateTime.UtcNow
            },
            new Villa
            {
                Id   = 2,
                Name = "Premium Pool Villa",
                Details =
                    "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                ImageUrl    = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa1.jpg",
                Occupancy   = 4,
                Rate        = 300,
                Sqft        = 550,
                Amenity     = "",
                CreatedDate = DateTime.UtcNow
            },
            new Villa
            {
                Id   = 3,
                Name = "Luxury Pool Villa",
                Details =
                    "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                ImageUrl    = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa4.jpg",
                Occupancy   = 4,
                Rate        = 400,
                Sqft        = 750,
                CreatedDate = DateTime.UtcNow,
                Amenity     = ""
            },
            new Villa
            {
                Id   = 4,
                Name = "Diamond Villa",
                Details =
                    "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                ImageUrl    = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa5.jpg",
                Occupancy   = 4,
                Rate        = 550,
                Sqft        = 900,
                CreatedDate = DateTime.UtcNow,
                Amenity     = ""
            },
            new Villa
            {
                Id   = 5,
                Name = "Diamond Pool Villa",
                Details =
                    "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                ImageUrl    = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa2.jpg",
                Occupancy   = 4,
                Rate        = 600,
                Sqft        = 1100,
                CreatedDate = DateTime.UtcNow,
                Amenity     = ""
            });
    }
}