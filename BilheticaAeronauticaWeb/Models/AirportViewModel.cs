using System.ComponentModel.DataAnnotations;
using BilheticaAeronauticaWeb.Entities;

namespace BilheticaAeronauticaWeb.Models
{
    public class AirportViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        public int CountryId { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid city.")]
        public int CityId { get; set; }
    }
}

