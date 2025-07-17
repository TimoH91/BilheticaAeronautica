using BilheticaAeronauticaWeb.Entities;

namespace BilheticaAeronauticaWeb.Models
{
    public class RoundTripFlightViewModel
    {
        public IEnumerable<Flight> OutboundFlights { get; set; }
        public IEnumerable<Flight>? ReturnFlights { get; set; }
    }
}
