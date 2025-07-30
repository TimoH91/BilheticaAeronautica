using System.Runtime.ExceptionServices;
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

        public async Task<Flight> GetByIdTrackedAsync(int id)
        {
            return await _context.Flights
             .FirstOrDefaultAsync(a => a.Id == id);
        }

        public override async Task<Flight> GetByIdAsync(int id)
        {
            return await _context.Flights
             .Include(a => a.Airplane)
             .Include(a => a.OriginAirport)
             .Include(a => a.DestinationAirport)
             .Include(a => a.Layover)
             .AsNoTracking()
             .FirstOrDefaultAsync(a => a.Id == id);
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

        public async Task<IEnumerable<Flight>> GetFlightsByAirplane(int airplaneId)
        {
            return await _context.Flights
                .Where(f => f.AirplaneId == airplaneId)
                .OrderBy(f => f.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Flight>> GetFlightsByOriginAndDestination(int originAirportId, int destinationAirportId)
        {
            var flights = await _context.Flights
                .Include(f => f.OriginAirport)
                .Include(f => f.DestinationAirport)
                .Include(f => f.Layover)
                .Where(f => f.OriginAirportId == originAirportId && f.DestinationAirportId == destinationAirportId)
                .OrderBy(f => f.Date)
                .ToListAsync();

            return flights;
        }

        public async Task<IEnumerable<Flight>> GetFlightsByOriginDestinationAndDate(int originAirportId, int destinationAirportId, DateTime date)
        {
            {
                var flights = await _context.Flights
                    .Include(f => f.OriginAirport)
                    .Include(f => f.DestinationAirport)
                    .Include(f => f.Layover)
                    .Where(f => f.OriginAirportId == originAirportId && f.DestinationAirportId == destinationAirportId && f.Date == date)
                    .OrderBy(f => f.Date)
                    .ToListAsync();

                return flights;
            }
        }

        public async Task<bool> IsAirportUsedInAnyFlightAsync(int airportId)
        {
                    return await _context.Flights
                 .AnyAsync(f =>
                f.OriginAirportId == airportId ||
                f.DestinationAirportId == airportId ||
                f.LayoverId == airportId);
        }
    }
}
