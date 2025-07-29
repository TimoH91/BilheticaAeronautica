using System.ComponentModel.DataAnnotations;
using BilheticaAeronauticaWeb.Entities;

namespace BilheticaAeronauticaWeb.Models
{
    public class AirplaneViewModel : Airplane
    {
        [Display(Name = "Photo")]
        public IFormFile? ImageFile { get; set; }
    }
}
