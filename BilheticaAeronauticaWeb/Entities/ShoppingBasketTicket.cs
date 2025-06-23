using System.ComponentModel.DataAnnotations;
using NuGet.ContentModel;

namespace BilheticaAeronauticaWeb.Entities
{
    public class ShoppingBasketTicket
    {
        public int Id { get; set; }

        public int ShoppingBasketId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        //[Required]
        //public Airport OriginAirport { get; set; }

        //[Required]
        //public Airport DestinationAirport { get; set; }

        [Required]
        public int SeatId { get; set; }

        [Required]
        public int FlightId { get; set; }

        [Required]
        public PassengerType PassengerType { get; set; }

        [Required]
        public TicketClass Class { get; set; }

        [Required]
        public decimal Price { get; set; }

        public int? ResponsibleAdultId { get; set; }

    }
}
