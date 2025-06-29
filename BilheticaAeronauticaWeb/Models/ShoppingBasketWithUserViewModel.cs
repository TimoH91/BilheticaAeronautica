using BilheticaAeronauticaWeb.Entities;

namespace BilheticaAeronauticaWeb.Models
{
    public class ShoppingBasketWithUserViewModel
    {
        public ShoppingBasket ShoppingBasket { get; set; } = new ShoppingBasket();

        public RegisterNewUserViewModel? NewUser { get; set; }
    }
}
