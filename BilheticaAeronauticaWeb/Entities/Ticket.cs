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

        //public Ticket(string name, string surname, int? flightId, Flight flight, TicketClass ticketClass, int? seatId, Seat seat, int originAirportId, Airport originAirport, int destinationAirportId, Airport destinationAirport, Payment payment, decimal price)
        //{
        //    Name = name;
        //    Surname = surname;
        //    FlightId = flightId;
        //    Flight = flight;
        //    Class = ticketClass;
        //    SeatId = seatId;
        //    Seat = seat;
        //    OriginAirportId = originAirportId;
        //    OriginAirport = originAirport;
        //    DestinationAirportId = destinationAirportId;
        //    DestinationAirport = destinationAirport;
        //    Payment = payment;
        //    Price = price;
        //}

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

        public Airport OriginAirport { get; set; }

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
