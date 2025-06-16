namespace BilheticaAeronauticaWeb.Entities
{
    public class ChildTicket : Ticket
    {
        public override string Type { get; set; } = "Child";

        //public ChildTicket(string name, string surname, int? flightId, Flight flight, TicketClass ticketClass, int? seatId, Seat seat, int originAirportId, Airport originAirport, int destinationAirportId, Airport destinationAirport, Payment payment, decimal price)
        //    : base(name, surname, flightId, flight, ticketClass, seatId, seat, originAirportId, originAirport, destinationAirportId, destinationAirport, payment, price)
        //{
        //    Type = "Child";
        //}
    }
}
