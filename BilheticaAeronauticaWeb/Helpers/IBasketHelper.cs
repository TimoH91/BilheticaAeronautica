using BilheticaAeronauticaWeb.Entities;

namespace BilheticaAeronauticaWeb.Helpers
{
    public interface IBasketHelper
    {
        void SaveBasket(ISession session, ShoppingBasket basket);

        ShoppingBasket GetBasket(ISession session);

        void ClearBasket(ISession session);
    }
}
