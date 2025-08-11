using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilheticaAeronautica.Mobile.Models
{
    public class Ticket
    {
        public string UserId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public int? FlightId { get; set; }

        public int? SeatId { get; set; }

        public int OriginAirportId { get; set; }

        public int DestinationAirportId { get; set; }

        public decimal Price { get; set; }

        public int OrderId { get; set; }

        public string FullName => $"{Name} {Surname}";
    }
}
