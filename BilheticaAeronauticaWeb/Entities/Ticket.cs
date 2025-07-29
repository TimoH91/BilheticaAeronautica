using System.ComponentModel;
using Newtonsoft.Json;
using NuGet.ContentModel;

namespace BilheticaAeronauticaWeb.Entities
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

    public enum Payment
    {
        Paid,
        Unpaid,
        Returned
    };
    public abstract class Ticket : IEntity
    {

        public string UserId { get; set; }
        public User User { get; set; }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public int? FlightId { get; set; }

        public Flight? Flight { get; set; }
        public TicketClass Class { get; set; }

        public int? SeatId { get; set; }

        public Seat? Seat { get; set; }

        [DisplayName("Origin")]
        public Airport OriginAirport { get; set; }

        [DisplayName("Destination")]
        public Airport DestinationAirport { get; set; }

        public int OriginAirportId { get; set; }

        public int DestinationAirportId { get; set; }

        public Payment Payment { get; set; }

        public decimal Price { get; set; }

        public abstract PassengerType Type { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}
