using BilheticaAeronauticaWeb.Entities;

namespace BilheticaAeronauticaWeb.Services
{
    public interface IOrderService
    {
        Task ClearShoppingBasket(ShoppingBasket shoppingBasket);
    }
}
