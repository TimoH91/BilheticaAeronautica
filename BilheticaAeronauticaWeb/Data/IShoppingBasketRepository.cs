using System.CodeDom;
using BilheticaAeronauticaWeb.Entities;
namespace BilheticaAeronauticaWeb.Data
{
    public interface IShoppingBasketRepository : IGenericRepository<ShoppingBasketTicket>
    {
        //Task<ShoppingBasket> AddTicketToShoppingBasket(ShoppingBasketTicket basketTicket, User? user);

        Task AddShoppingBasketTicket(ShoppingBasketTicket basketTicket);

        //Task<ShoppingBasket> GetShoppingBasketAsync(string userName);

        Task<ShoppingBasketTicket> GetShoppingBasketTicketAsync(int id);

        Task DeleteShoppingBasketTickets(IEnumerable<ShoppingBasketTicket> tickets);

        Task<List<ShoppingBasketTicket>> GetShoppingBasketTicketsAsync(User user);



    }
}
