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
            return _context.Tickets.Include(t => t.DestinationAirport).Include(t => t.Flight).Include(t => t.OriginAirport).Include(t => t.Seat);
        }
    }
}
