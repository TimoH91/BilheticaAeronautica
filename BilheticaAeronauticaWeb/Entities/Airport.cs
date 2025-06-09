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
        public string City { get; set; }

        [Required] 
        [MaxLength(100, ErrorMessage = "The field {0} cannot contain more than {1} characters length.")]
        public string Country { get; set; }

        [Display(Name = "Image")]
        public Guid FlagImageId { get; set; }

        //TODO: get another 30 days on azure and get blob storage link

        public string FlagImageFullPath => FlagImageId == Guid.Empty
            ? "https://supershop04-dya4b0endqhyc3db.westeurope-01.azurewebsites.net/images/noimage.jpg"
        : $"https://flagcdn.com/w320/fr.png";
    }
}
