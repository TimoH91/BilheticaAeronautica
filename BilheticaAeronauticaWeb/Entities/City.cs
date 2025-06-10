using System.ComponentModel.DataAnnotations;

namespace BilheticaAeronauticaWeb.Entities
{
    public class City : IEntity 
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "City")]
        [MaxLength(50, ErrorMessage = "The field {0} can contain {1} characters.")]
        public string Name { get; set; }

        public ICollection<Airport> Airports { get; set; }
    }
}
