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

        public City City { get; set; }

        public Country Country { get; set; }

        
    }
}
