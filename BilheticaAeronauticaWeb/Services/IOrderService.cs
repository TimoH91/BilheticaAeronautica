using BilheticaAeronauticaWeb.Entities;
using BilheticaAeronauticaWeb.Models;

namespace BilheticaAeronauticaWeb.Services
{
    public interface IOrderService
    {
        Task ClearShoppingBasket(List<ShoppingBasketTicket> ShoppingBasketTickets);

        Task ClearShoppingBasketByUser(User user);

        Order ConvertToOrder(List<ShoppingBasketTicket> basketTickets, User user);

        Task<List<Ticket>> ConvertAdultAndChildTickets(List<ShoppingBasketTicket> shoppingBasketTickets, User user);

        Task<List<Ticket>> AddTicketsAsync(List<Ticket> tickets, Order order);

        Task<List<Ticket>> ConvertInfantTickets(List<Ticket> tickets, List<ShoppingBasketTicket> shoppingBasketTickets, User user);
    }
}
