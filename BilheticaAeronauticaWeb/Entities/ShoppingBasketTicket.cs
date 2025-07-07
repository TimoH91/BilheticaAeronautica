using System.ComponentModel.DataAnnotations;
using NuGet.ContentModel;

namespace BilheticaAeronauticaWeb.Entities
{
    public class ShoppingBasketTicket : IEntity
    {
        public int Id { get; set; }

        //public int ShoppingBasketId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        //[Required]
        //public int OriginAirportId { get; set; }

        //[Required]
        //public int DestinationAirportId { get; set; }

        [Required]
        public Seat? Seat { get; set; }

        [Required]
        public Flight Flight { get; set; }

        [Required]
        public PassengerType PassengerType { get; set; }

        [Required]
        public TicketClass Class { get; set; }

        [Required]
        public decimal Price { get; set; }

        public int? ResponsibleAdultId { get; set; }

        public User? User { get; set; }


    }
}
