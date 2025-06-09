using System.ComponentModel.DataAnnotations;
using BilheticaAeronauticaWeb.Entities;

namespace BilheticaAeronauticaWeb.Models
{
    public class AirportViewModel : Airport
    {
        
        [Display(Name = "Flag")]
        public IFormFile ImageFile { get; set; }
        
    }
}

