using System.ComponentModel;

namespace BilheticaAeronauticaWeb.Entities
{
    public class Order : IEntity
    {
        public int Id { get; set; }

        public User User { get; set; }

        [DisplayName("Order Date")]
        public DateTime OrderDate { get; set; }

        [DisplayName("Total Price")]
        public decimal TotalPrice { get; set; }

        public Payment Payment { get; set; }

        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
