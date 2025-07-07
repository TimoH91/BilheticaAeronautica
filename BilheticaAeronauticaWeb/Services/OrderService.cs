using System.Threading.Tasks;
using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Entities;

namespace BilheticaAeronauticaWeb.Services
{
    public class OrderService : IOrderService
    {
        private readonly IShoppingBasketRepository _shoppingBasketRepository;
        public OrderService(IShoppingBasketRepository shoppingBasketRepository, DataContext context)
        {
            _shoppingBasketRepository = shoppingBasketRepository;
        }

        public async Task ClearShoppingBasket(List<ShoppingBasketTicket> shoppingBasketTickets)
        {
            await _shoppingBasketRepository.DeleteShoppingBasketTickets(shoppingBasketTickets);
            //await _shoppingBasketRepository.DeleteAsync(shoppingBasket);
        }
    }
}
