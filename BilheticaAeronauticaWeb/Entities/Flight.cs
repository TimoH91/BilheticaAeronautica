using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace BilheticaAeronauticaWeb.Entities
{
    public class Flight : IEntity
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "The field {0} is required.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }


        [Required(ErrorMessage = "The field {0} is required.")]
        public TimeSpan Time { get; set; }

        [Required(ErrorMessage = "The field {0} is required.")]
        public int Duration { get; set; }

        [Required(ErrorMessage = "The field {0} is required.")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        [DisplayName("Base Price")]
        public decimal BasePrice { get; set; }

        public Airplane Airplane { get; set; }

        [DisplayName("Origin")]
        public Airport OriginAirport { get; set; }

        [DisplayName("Destination")]
        public Airport DestinationAirport { get; set; }

        public Airport? Layover { get; set; }

        public int AirplaneId { get; set; }

        public int OriginAirportId { get; set; }

        public int DestinationAirportId { get; set; }

        public int? LayoverId { get; set; }

    }
}
