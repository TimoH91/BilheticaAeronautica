using System.ComponentModel.DataAnnotations;
using BilheticaAeronauticaWeb.Entities;
using BilheticaAeronauticaWeb.Validations;

namespace BilheticaAeronauticaWeb.Models
{
    public class FlightViewModel
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [FutureDateValidation(ErrorMessage = "Date must be in the future.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Required]
        public TimeSpan Time { get; set; }


        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Duration must be greater than 0.")]
        public int Duration { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal BasePrice { get; set; }

        [Required(ErrorMessage = "You must select an airplane.")]
        public int AirplaneId { get; set; }
        [Required(ErrorMessage = "You must select an origin airport.")]
        public int OriginAirportId { get; set; }
        [Required(ErrorMessage = "You must select a destination airport.")]
        public int DestinationAirportId { get; set; }

        public int? LayoverId { get; set; }
    }
}
