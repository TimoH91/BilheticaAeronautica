using System.ComponentModel.DataAnnotations;
using BilheticaAeronauticaWeb.Entities;

namespace BilheticaAeronauticaWeb.Models
{
    public class CountryViewModel : Country
    {
        [Display(Name = "Flag")]
        public IFormFile ImageFile { get; set; }
    }
}
