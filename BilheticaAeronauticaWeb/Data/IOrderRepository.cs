using BilheticaAeronauticaWeb.Entities;

namespace BilheticaAeronauticaWeb.Data
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<ShoppingBasket> AddTicketToShoppingBasket(ShoppingBasketTicket basketTicket, string userName);
    }
}
