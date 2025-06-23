using BilheticaAeronauticaWeb.Entities;
using BilheticaAeronauticaWeb.Helpers;
using BilheticaAeronauticaWeb.Models;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;

namespace BilheticaAeronauticaWeb.Data
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IConverterHelper _converterHelper;


        public OrderRepository(DataContext context, IUserHelper userHelper, IConverterHelper converterHelper) : base(context)
        {
            _context = context;
            _userHelper = userHelper;
            _converterHelper = converterHelper;
        }

        public async Task<ShoppingBasket> AddTicketToShoppingBasket(ShoppingBasketTicket basketTicket, string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);

            if (user == null)
            {
                throw new ArgumentException("User not deteceted");
            }

            var shoppingBasket = await _context.ShoppingBaskets.Where(s => s.User == user)
                .FirstOrDefaultAsync();


            if (shoppingBasket == null) 
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
                basketTicket.ShoppingBasketId = shoppingBasket.Id;
                _context.ShoppingBasketTickets.Add(basketTicket);

                await _context.SaveChangesAsync();

                return shoppingBasket;
            }
        }

        //public async Task AddTicketToBasketAsync(TicketViewModel model, string userName)
        //{
        //    var user = await _userHelper.GetUserByEmailAsync(userName);

        //    if (user == null)
        //    {
        //        return;

        //    }

        //    var shoppingBasketTicket = _converterHelper.ToShoppingBasketTicket(model, true);

        //    //var orderBasketTicket = await _context.ShoppingBasketTickets.Where(odt => odt.User == user && odt.Product == product)
        //        //.FirstOrDefaultAsync();


        //    if ((shoppingBasketTicket == null))
        //    {
        //        return;
        //    }




        //    await _context.SaveChangesAsync();
        //}
    }
}
