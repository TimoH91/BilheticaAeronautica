using BilheticaAeronauticaWeb.Entities;
using BilheticaAeronauticaWeb.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BilheticaAeronauticaWeb.Data
{
    public class ShoppingBasketRepository : GenericRepository<ShoppingBasket>, IShoppingBasketRepository
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

        public async Task<ShoppingBasket> AddTicketToShoppingBasket(ShoppingBasketTicket basketTicket, User? user)
        {
            //var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

            if (user != null)
            {
                var shoppingBasket2 = await _context.ShoppingBaskets.Where(s => s.User == user)
               .FirstOrDefaultAsync();

                if (shoppingBasket2 == null)
                {
                    var shoppingBasket1 = new ShoppingBasket
                    {
                        User = user,
                    };

                    _context.ShoppingBaskets.Add(shoppingBasket1);
                    await _context.SaveChangesAsync();

                    basketTicket.ShoppingBasketId = shoppingBasket1.Id;
                    _context.ShoppingBasketTickets.Add(basketTicket);

                    await _context.SaveChangesAsync();

                    return shoppingBasket1;
                }
                else
                {
                    basketTicket.ShoppingBasketId = shoppingBasket2.Id;
                    _context.ShoppingBasketTickets.Add(basketTicket);

                    await _context.SaveChangesAsync();

                    return shoppingBasket2;
                }
            }

            var shoppingBasket = new ShoppingBasket();
            shoppingBasket.Tickets.Add(basketTicket);

            return shoppingBasket;
        }

        public async Task DeleteShoppingBasketTickets(IEnumerable<ShoppingBasketTicket> tickets)
        {
                _context.ShoppingBasketTickets.RemoveRange(tickets);
                await _context.SaveChangesAsync();
        }

        public async Task<ShoppingBasket> GetShoppingBasketAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);

            if (user == null)
            {
                return new ShoppingBasket();
            }

            if (await _userHelper.IsUserInRoleAsync(user, "Customer"))
            {
                return _context.ShoppingBaskets.
                    Include(o => o.User).
                    Include(o => o.Tickets).
                    FirstOrDefault(o => o.User == user);
            }

            return null;
        }

        public async Task<ShoppingBasketTicket> GetShoppingBasketTicketAsync(int id)
        {
            return await _context.ShoppingBasketTickets
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}
