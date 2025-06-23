
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace BilheticaAeronauticaWeb.Entities
{
    public class ShoppingBasket : IEntity
    {
            public int Id { get ; set; }

            [Required]
            public User User { get; set; } 

            public ICollection<ShoppingBasketTicket> Tickets { get; set; } = new List<ShoppingBasketTicket>();
    }
}
