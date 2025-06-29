using System.Threading.Tasks;
using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Entities;

namespace BilheticaAeronauticaWeb.Services
{
    public class OrderService : IOrderService
    {
        private readonly IShoppingBasketRepository _shoppingBasketRepository;
        private readonly DataContext _context;
        public OrderService(IShoppingBasketRepository shoppingBasketRepository, DataContext context)
        {
            _shoppingBasketRepository = shoppingBasketRepository;
            _context = context;
        }

        public async Task ClearShoppingBasket(ShoppingBasket shoppingBasket)
        {
            await _shoppingBasketRepository.DeleteShoppingBasketTickets(shoppingBasket.Tickets);
            await _shoppingBasketRepository.DeleteAsync(shoppingBasket);
            await _context.SaveChangesAsync();
        }
    }
}
