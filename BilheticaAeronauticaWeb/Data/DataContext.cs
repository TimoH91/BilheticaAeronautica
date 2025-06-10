using Microsoft.EntityFrameworkCore;
using BilheticaAeronauticaWeb.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BilheticaAeronauticaWeb.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DbSet<Airport> Airports { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<City> Cities { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
    }
}
