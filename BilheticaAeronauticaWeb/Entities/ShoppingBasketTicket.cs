using System.ComponentModel.DataAnnotations;
using NuGet.ContentModel;

namespace BilheticaAeronauticaWeb.Entities
{
    public class ShoppingBasketTicket : IEntity
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        //[Required]
        //public int OriginAirportId { get; set; }

        //[Required]
        //public int DestinationAirportId { get; set; }

        public int? SeatId { get; set; }

        public Seat? Seat { get; set; }

        [Required]
        public Flight Flight { get; set; }


        public int FlightId { get; set; }

        [Required]
        public PassengerType PassengerType { get; set; }

        [Required]
        public TicketClass Class { get; set; }

        [Required]
        public decimal Price { get; set; }

        public int? ResponsibleAdultTicketId { get; set; }

        public bool IsResponsibleAdult { get; set; }

        public string? UserId { get; set; }

        public User? User { get; set; }


    }
}
