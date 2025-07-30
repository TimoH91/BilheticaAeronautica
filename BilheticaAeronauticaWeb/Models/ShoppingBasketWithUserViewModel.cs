using BilheticaAeronauticaWeb.Entities;

namespace BilheticaAeronauticaWeb.Models
{
    public class ShoppingBasketWithUserViewModel
    {
        public List<ShoppingBasketTicketViewModel> ShoppingBasketTickets { get; set; } = new List<ShoppingBasketTicketViewModel>();

        public RegisterNewUserViewModel? NewUser { get; set; }
    }
}
