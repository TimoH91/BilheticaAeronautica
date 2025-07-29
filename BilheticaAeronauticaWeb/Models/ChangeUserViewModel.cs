using System.ComponentModel;
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

        [DisplayName("Photo")]
        public Guid ImageId { get; set; }

        public string ImageFullPath => ImageId == Guid.Empty
            ? "~/images/noimage.jpg"
        : $"https://brisa.blob.core.windows.net/users/{ImageId}";
    }
}
