using BilheticaAeronauticaWeb.Entities;
using BilheticaAeronauticaWeb.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BilheticaAeronauticaWeb.Data
{
    public class ShoppingBasketRepository : GenericRepository<ShoppingBasketTicket>, IShoppingBasketRepository
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IConverterHelper _converterHelper;


        public ShoppingBasketRepository(DataContext context, IUserHelper userHelper, IConverterHelper converterHelper) : base(context)
        {
            _context = context;
            _userHelper = userHelper;
            _converterHelper = converterHelper;
        }

        public async Task AddShoppingBasketTicket(ShoppingBasketTicket basketTicket)
        {
            _context.ShoppingBasketTickets.Add(basketTicket);
            await _context.SaveChangesAsync();
        }


        public async Task DeleteShoppingBasketTickets(IEnumerable<ShoppingBasketTicket> tickets)
        {
                _context.ShoppingBasketTickets.RemoveRange(tickets);
                await _context.SaveChangesAsync();
        }

        public async Task<ShoppingBasketTicket> GetShoppingBasketTicketAsync(int id)
        {
            return await _context.ShoppingBasketTickets
                 .Include(a => a.Flight)
                .ThenInclude(a => a.DestinationAirport)
                 .Include(a => a.Flight)
                .ThenInclude(a => a.OriginAirport)
                .Include(a => a.Seat)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<ShoppingBasketTicket>> GetShoppingBasketTicketsAsync(User user)
        {
            return await _context.ShoppingBasketTickets
                .Where(o => o.User.Id == user.Id)
                .Include(a => a.Flight)
                .ThenInclude(a => a.DestinationAirport)
                 .Include(a => a.Flight)
                .ThenInclude(a => a.OriginAirport)
                .Include(a => a.Seat)
                .ToListAsync();
        }

    }
}
