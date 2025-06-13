using System.Threading.Tasks;
using BilheticaAeronauticaWeb.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BilheticaAeronauticaWeb.Data
{
    public class FlightRepository : GenericRepository<Flight>, IFlightRepository
    {
        private readonly DataContext _context;

        public FlightRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public override IQueryable<Flight> GetAll()
        {
            return _context.Flights
                .Include(a => a.Airplane)
                .Include(a => a.OriginAirport)
                .Include(a => a.DestinationAirport)
                .Include(a => a.Layover);
        }




    }
}
