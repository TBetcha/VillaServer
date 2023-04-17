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
}