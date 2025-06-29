using BilheticaAeronauticaWeb.Entities;

namespace BilheticaAeronauticaWeb.Data
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersByUserAsync(string userName);
    }
}
