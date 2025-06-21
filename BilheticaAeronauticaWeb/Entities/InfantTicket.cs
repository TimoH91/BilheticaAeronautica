namespace BilheticaAeronauticaWeb.Entities
{
    public class InfantTicket : Ticket
    {
        public override PassengerType Type { get; set; } = PassengerType.Infant;

        public int ResponsibleAdultId { get; set; }

        //public InfantTicket(string name, string surname, int? flightId, Flight flight, TicketClass ticketClass, int? seatId, Seat seat, int originAirportId, Airport originAirport, int destinationAirportId, Airport destinationAirport, Payment payment, decimal price, int responsibleAdultId)
        //    : base(name, surname, flightId, flight, ticketClass, seatId, seat, originAirportId, originAirport, destinationAirportId, destinationAirport, payment, price)
        //{
        //    Type = "Infant";
        //    ResponsibleAdultId = responsibleAdultId;
        //}
    }
}
