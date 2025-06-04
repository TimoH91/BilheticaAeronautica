using System.ComponentModel.DataAnnotations;

namespace BilheticaAeronauticaWeb.Entities
{
    public class Aeroporto : IEntity
    {
        public int Id { get; set; }

        [Required] 
        [MaxLength(100, ErrorMessage = "The field {0} cannot contain more than {1} characters length.")]
        public string Nome { get; set; }

        [Required] 
        [MaxLength(100, ErrorMessage = "The field {0} cannot contain more than {1} characters length.")]
        public string Cidade { get; set; }

        [Required] 
        [MaxLength(100, ErrorMessage = "The field {0} cannot contain more than {1} characters length.")]
        public string Pais { get; set; }    
    }
}
