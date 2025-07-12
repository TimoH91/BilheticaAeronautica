using BilheticaAeronauticaWeb.Entities;
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
            return _context.Tickets.Include(t => t.DestinationAirport).Include(t => t.Flight).Include(t => t.OriginAirport).Include(t => t.Seat).Include(t => t.UserId);
        }

        public async Task<Ticket> GetTicketBySeatIdAsync(int seatId, string firstName, string lastName)
        {
            return await _context.Tickets
                .FirstOrDefaultAsync(t => t.SeatId == seatId && t.Name == firstName && t.Surname == lastName);
        }

        public override async Task<Ticket> GetByIdAsync(int id)
        {
            return await _context.Tickets
                .Include(t => t.DestinationAirport)
                .Include(t => t.OriginAirport)
                .Include(t => t.Flight)
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

        //public override async Task CreateRangeAsync(IEnumerable<Ticket> tickets)
        //{
        //    foreach (var ticket in tickets)
        //    {
        //        // This ensures no tracking conflict — assumes Flight is already in DB
        //        if (ticket.Flight != null)
        //        {
        //            _context.Entry(ticket.Flight).State = EntityState.Unchanged;
        //            ticket.FlightId = ticket.Flight.Id; // optional but explicit
        //            ticket.Flight = null; // prevent EF from thinking it's a new entity
        //        }

        //        if (ticket.Order != null)
        //        {
        //            _context.Entry(ticket.Order).State = EntityState.Unchanged;
        //            ticket.OrderId = ticket.Order.Id;
        //            ticket.Order = null;
        //        }
        //    }

        //    await base.CreateRangeAsync(tickets); // calls your generic implementation
        //}
    }
}
