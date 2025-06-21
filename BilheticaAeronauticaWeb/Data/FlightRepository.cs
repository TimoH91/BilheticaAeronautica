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

        public IEnumerable<SelectListItem> GetComboFlights()
        {
            var list = _context.Flights
                .Include(f => f.OriginAirport)
                .Include(f => f.DestinationAirport)
                .AsEnumerable() 
                .Select(a => new SelectListItem
                {
                    Text = $"{a.OriginAirport.Name} to {a.DestinationAirport.Name}",
                    Value = a.Id.ToString()
                })
                .OrderBy(l => l.Text)
                .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a flight...)",
                Value = "0"
            });

            return list;
        }

        public async Task<IEnumerable<object>> GetFlightsByOriginAndDestination(int originAirportId, int destinationAirportId)
        {
            var flights = await _context.Flights
                .Include(f => f.OriginAirport)
                .Include(f => f.DestinationAirport)
                .Where(f => f.OriginAirportId == originAirportId && f.DestinationAirportId == destinationAirportId)
                .Select(f => new {
                    id = f.Id,
                    name = f.OriginAirport.Name + " to " + f.DestinationAirport.Name,
                    price = f.BasePrice
                })
                .OrderBy(f => f.name)
                .ToListAsync();

            return flights;
        }
    }
}
