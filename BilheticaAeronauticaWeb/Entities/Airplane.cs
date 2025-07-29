using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BilheticaAeronauticaWeb.Entities
{
    public class Airplane : IEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "The field {0} cannot contain more than {1} characters length.")]
        public string Name { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "The field {0} cannot contain more than {1} characters length.")]
        public string Manufacturer { get; set; }

        [Required(ErrorMessage = "The field {0} is required.")]
        [Range(1, 100, ErrorMessage = "The field {0} must be between {1} and {2}.")]
        public int Rows { get; set; }

        [DisplayName("Seats per row")]
        [Required(ErrorMessage = "The field {0} is required.")]
        [Range(1, 26, ErrorMessage = "The field {0} must be between {1} and {2}.")]
        public int SeatsPerRow { get; set; }

        [DisplayName("Active")]
        public bool Status { get; set; }

        [DisplayName("Photo")]
        public Guid ImageId { get; set; }

        public string ImageFullPath => ImageId == Guid.Empty
            ? "~/images/noimage.jpg"
        : $"https://brisa.blob.core.windows.net/airplanes/{ImageId}";
    }
}
