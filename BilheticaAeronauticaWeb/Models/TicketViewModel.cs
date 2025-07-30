using System.ComponentModel;
using BilheticaAeronauticaWeb.Entities;

namespace BilheticaAeronauticaWeb.Models
{
    public class TicketViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public int? FlightId { get; set; }
        public TicketClass Class { get; set; }

        public int? SeatId { get; set; }

        [DisplayName("Origin")]
        public int OriginAirportId { get; set; }

        [DisplayName("Destination")]
        public int DestinationAirportId { get; set; }

        [DisplayName("Destination")]

        //public Payment Payment { get; set; }

        public int? OrderId { get; set; }

        public decimal Price { get; set; }

        public PassengerType Type { get; set; }

        [DisplayName("Ticket Id of Responsible Adult")]
        public int? ResponsibleAdultTicketId { get; set; }
    }
}
