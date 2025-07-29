using BilheticaAeronauticaWeb.Entities;
using System.ComponentModel;

namespace BilheticaAeronauticaWeb.Models
{
    public class ShoppingBasketTicketViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public int? SeatId { get; set; }

        public int FlightId { get; set; }

        [DisplayName("Passenger type")]
        public PassengerType PassengerType { get; set; }

        public TicketClass Class { get; set; }

        public decimal Price { get; set; }

        public int? ResponsibleAdultTicketId { get; set; }

        public string? UserId { get; set; }
    }
}
