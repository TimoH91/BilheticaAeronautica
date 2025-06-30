using Microsoft.AspNetCore.Identity;

namespace BilheticaAeronauticaWeb.Entities
{
    public class User : IdentityUser
    {
            public string FirstName { get; set; }

            public string LastName { get; set; }

            //public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    }
}
