using Microsoft.EntityFrameworkCore;
using BilheticaAeronauticaWeb.Entities;

namespace BilheticaAeronauticaWeb.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Aeroporto> Aeroportos { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
    }
}
