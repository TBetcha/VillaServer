using Microsoft.EntityFrameworkCore;
using VillaAPI.Models;

namespace VillaAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Villa> Villas { get; set; }
}