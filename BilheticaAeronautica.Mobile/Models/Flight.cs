using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilheticaAeronautica.Mobile.Models
{
    public class Flight
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public TimeSpan Time { get; set; }

        public int Duration { get; set; }

        public decimal BasePrice { get; set; }

        public int AirplaneId { get; set; }

        //public string OriginAirport { get; set; }

        //public int OriginAirportId { get; set; }

        //public string DestinationAirport { get; set; }
        //public int DestinationAirportId { get; set; }

        //public int? LayoverId { get; set; }
    }

}
