using System.CodeDom;
using BilheticaAeronauticaWeb.Entities;
namespace BilheticaAeronauticaWeb.Data
{
    public interface IShoppingBasketRepository : IGenericRepository<ShoppingBasket>
    {
        Task<ShoppingBasket> AddTicketToShoppingBasket(ShoppingBasketTicket basketTicket, User? user);

        Task<ShoppingBasket> GetShoppingBasketAsync(string userName);

        Task<ShoppingBasketTicket> GetShoppingBasketTicketAsync(int id);

        Task DeleteShoppingBasketTickets(IEnumerable<ShoppingBasketTicket> tickets);
    }
}
