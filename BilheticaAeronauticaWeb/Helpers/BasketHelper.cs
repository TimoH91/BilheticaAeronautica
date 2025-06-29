using BilheticaAeronauticaWeb.Entities;
using NuGet.ContentModel;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using BilheticaAeronauticaWeb.Models;

namespace BilheticaAeronauticaWeb.Helpers
{
    public class BasketHelper : IBasketHelper
    {
        private const string BasketSessionKey = "TemporaryBasket";

        public void ClearBasket(ISession session)
        {
            session.Remove(BasketSessionKey);
        }

        public ShoppingBasket GetBasket(ISession session)
        {
            var json = session.GetString(BasketSessionKey);

            if (string.IsNullOrEmpty(json))
            {
                return new ShoppingBasket();
            }

            return JsonSerializer.Deserialize<ShoppingBasket>(json);
        }

        public void SaveBasket(ISession session, ShoppingBasket basket)
        {
            var json = JsonSerializer.Serialize(basket);
            session.SetString(BasketSessionKey, json);
        }
    }
}
