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
            foreach (var basketTicket in shoppingBasketTickets)
            {
                basketTicket.Flight = null;
                basketTicket.Seat = null;
                await _shoppingBasketRepository.DeleteAsync(basketTicket);
            }
            //await _shoppingBasketRepository.DeleteAsync(shoppingBasket);
        }

        public async Task ClearShoppingBasketByUser(User user)
        {
            var basketTickets = await _shoppingBasketRepository.GetShoppingBasketTicketsAsync(user);

            foreach (var basketTicket in basketTickets)
            {
                //basketTicket.Flight = null;
                //basketTicket.Seat = null;
                await _shoppingBasketRepository.DeleteAsync(basketTicket);
            }

        }

    }
}
