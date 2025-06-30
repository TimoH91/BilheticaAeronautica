using BilheticaAeronauticaWeb.Entities;

namespace BilheticaAeronauticaWeb.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }

        public Payment Payment { get; set; }

        public List<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
