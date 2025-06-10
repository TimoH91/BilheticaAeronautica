using BilheticaAeronauticaWeb.Entities;
using Microsoft.EntityFrameworkCore;

namespace BilheticaAeronauticaWeb.Data
{
    public class AirportRepository : GenericRepository<Airport>, IAirportRepository
    {
        private readonly DataContext _context;

        public AirportRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public override IQueryable<Airport> GetAll()
        {
            return _context.Airports
                .Include(a => a.City)
                .Include(a => a.Country);
        }
    }
}
