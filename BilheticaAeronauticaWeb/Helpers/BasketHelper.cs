using BilheticaAeronauticaWeb.Entities;
using NuGet.ContentModel;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using BilheticaAeronauticaWeb.Models;
using Org.BouncyCastle.Bcpg;

namespace BilheticaAeronauticaWeb.Helpers
{
    public class BasketHelper : IBasketHelper
    {
        private const string BasketSessionKey = "TemporaryBasket";

        public void ClearBasket(ISession session)
        {
            session.Remove(BasketSessionKey);
        }

        //public ShoppingBasket GetBasket(ISession session)
        //{
        //    var json = session.GetString(BasketSessionKey);

        //    if (string.IsNullOrEmpty(json))
        //    {
        //        return new ShoppingBasket();
        //    }

        //    return JsonSerializer.Deserialize<ShoppingBasket>(json);
        //}

        public List<ShoppingBasketTicket> GetBasketTickets(ISession session)
        {
            var json = session.GetString(BasketSessionKey);

            if (string.IsNullOrEmpty(json))
            {
                return new List<ShoppingBasketTicket>();
            }

            return JsonSerializer.Deserialize<List<ShoppingBasketTicket>>(json);
        }

        public void SaveBasketTickets(ISession session, List<ShoppingBasketTicket> Tickets)
        {
            var json = JsonSerializer.Serialize(Tickets);
            session.SetString(BasketSessionKey, json);
        }

        //public void SaveBasket(ISession session, ShoppingBasket basket)
        //{
        //    var json = JsonSerializer.Serialize(basket);
        //    session.SetString(BasketSessionKey, json);
        //}
    }
}
