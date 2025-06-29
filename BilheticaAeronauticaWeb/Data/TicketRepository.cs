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
    }
}
