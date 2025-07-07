using BilheticaAeronauticaWeb.Entities;

namespace BilheticaAeronauticaWeb.Models
{
    public class ShoppingBasketWithUserViewModel
    {
        //public ShoppingBasket ShoppingBasket { get; set; } = new ShoppingBasket();

        public List<ShoppingBasketTicket> ShoppingBasketTickets { get; set; } = new List<ShoppingBasketTicket>();

        public RegisterNewUserViewModel? NewUser { get; set; }
    }
}
