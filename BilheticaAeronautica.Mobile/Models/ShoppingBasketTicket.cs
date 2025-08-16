using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilheticaAeronautica.Mobile.Models
{
    public enum PassengerType
    {
        Adult,
        Child,
        Infant
    };
    public enum TicketClass
    {
        Economic,
        Business
    };
    public class ShoppingBasketTicket
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Surname { get; set; }

        public int? SeatId { get; set; }
        public int FlightId { get; set; }

        public PassengerType PassengerType { get; set; }
        public TicketClass Class { get; set; }
        public decimal Price { get; set; }

        public int? ResponsibleAdultTicketId { get; set; }
        public bool IsResponsibleAdult { get; set; }

        public string? UserId { get; set; }
    }
}
