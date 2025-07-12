using BilheticaAeronauticaWeb.Entities;

namespace BilheticaAeronauticaWeb.Services
{
    public interface IOrderService
    {
        Task ClearShoppingBasket(List<ShoppingBasketTicket> ShoppingBasketTickets);

        Task ClearShoppingBasketByUser(User user);
    }
}
