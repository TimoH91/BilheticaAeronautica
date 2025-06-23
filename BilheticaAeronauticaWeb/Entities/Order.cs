namespace BilheticaAeronauticaWeb.Entities
{
    public class Order : IEntity
    {
        public int Id { get; set; }

        public User User { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }

        public Payment Payment { get; set; }

        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
