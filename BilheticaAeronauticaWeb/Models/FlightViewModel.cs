using System.ComponentModel.DataAnnotations;
using BilheticaAeronauticaWeb.Entities;

namespace BilheticaAeronauticaWeb.Models
{
    public class FlightViewModel
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public TimeSpan Time { get; set; }

        public int Duration { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal BasePrice { get; set; }

        public int AirplaneId { get; set; }

        public int OriginAirportId { get; set; }

        public int DestinationAirportId { get; set; }

        public int? LayoverId { get; set; }
    }
}
