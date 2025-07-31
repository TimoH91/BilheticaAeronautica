using BilheticaAeronauticaWeb.Entities;
using BilheticaAeronauticaWeb.Migrations;
using Microsoft.EntityFrameworkCore;

namespace BilheticaAeronauticaWeb.Data
{
    public class TicketRepository : GenericRepository<Ticket>, ITicketRepository
    {
        private readonly DataContext _context;

        public TicketRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public override IQueryable<Ticket> GetAll()
        {
            return _context.Tickets.Include(t => t.DestinationAirport).Include(t => t.Flight)
                .Include(t => t.OriginAirport)
                .Include(t => t.Seat)
                .Include(t => t.User);
        }

        public async Task<Ticket> GetTicketBySeatIdAsync(int seatId, string firstName, string lastName)
        {
            return await _context.Tickets
                .FirstOrDefaultAsync(t => t.SeatId == seatId && t.Name == firstName && t.Surname == lastName);
        }

        public override async Task<Ticket> GetByIdAsync(int id)
        {
            return await _context.Tickets
                .Include(t => t.User)
                .Include(t => t.DestinationAirport)
                .Include(t => t.OriginAirport)
                .Include(t => t.Flight)
                .ThenInclude(t => t.Layover)
                .Include(t => t.Seat)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<Ticket>> GetTicketByOrderAsync(Order order)
        {
            return await _context.Tickets
               .Include(t => t.Flight)
               .Include(t => t.Seat)
               .Where(t => t.Order.Id == order.Id)
               .ToListAsync();
        }

        public async Task<List<Ticket>> GetTicketsByFlightIdAsync(int flightId)
        {
            return await _context.Tickets
              .Include(t => t.Flight)
              //.Include(t => t.Seat)
              .Include(t => t.Order)
              .Include(t => t.DestinationAirport)
              .Include(t => t.OriginAirport)
              .Where(t => t.FlightId == flightId)
              .ToListAsync();
        }

        public IQueryable GetAllWithUsers()
        {
            return _context.Tickets.Include(p => p.User);
        }

        public async Task<Ticket> GetTicketsByUser(User user)
        {
            return await _context.Tickets.FirstOrDefaultAsync(t => t.User == user);
        }

        public async Task<IEnumerable<Ticket>> GetFutureTickets(User user)
        {
             var tickets = await _context.Tickets
            .Include(t => t.Flight)
            .Where(t => t.UserId == user.Id)
            .ToListAsync();

            var futureTickets = tickets.Where(f => f.Flight != null && f.Flight.Date > DateTime.Now.Date
            || f.Flight != null && f.Flight.Date == DateTime.Now.Date && f.Flight.Time > DateTime.Now.TimeOfDay).ToList();

            return futureTickets;
        }

        public async Task<bool> IsAirportOnTicketAsync(int airportId)
        {
            return await _context.Tickets
         .AnyAsync(f => f.OriginAirportId == airportId || f.DestinationAirportId == airportId);

        }
    }

    
}
