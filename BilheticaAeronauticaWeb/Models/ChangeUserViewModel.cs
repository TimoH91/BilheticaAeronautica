using System.ComponentModel.DataAnnotations;

namespace BilheticaAeronauticaWeb.Models
{
    public class ChangeUserViewModel
    {
        [Required] 
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Photo")]
        public IFormFile ImageFile { get; set; }
    }
}
