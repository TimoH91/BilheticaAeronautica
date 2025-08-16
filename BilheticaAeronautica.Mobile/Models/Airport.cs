using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilheticaAeronautica.Mobile.Models
{
    public class Airport
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "The field {0} cannot contain more than {1} characters length.")]
        public string Name { get; set; }

        public int CityId { get; set; }
        public string CityName { get; set; }

        public int CountryId { get; set; }
        public string CountryName { get; set; }
    }
}
