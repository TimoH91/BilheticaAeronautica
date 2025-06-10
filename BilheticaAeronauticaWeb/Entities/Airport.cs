using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BilheticaAeronauticaWeb.Entities
{
    public class Airport : IEntity
    {
        public int Id { get; set; }

        [Required] 
        [MaxLength(100, ErrorMessage = "The field {0} cannot contain more than {1} characters length.")]
        public string Name { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "The field {0} cannot contain more than {1} characters length.")]
        public City City { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "The field {0} cannot contain more than {1} characters length.")]
        public Country Country { get; set; }

        
    }
}
