using System.ComponentModel.DataAnnotations;

namespace BilheticaAeronauticaWeb.Entities
{
    public class Country : IEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(60, ErrorMessage = "The field {0} can contain {1} characters.")]
        public string Name { get; set; }

        public ICollection<City>? Cities { get; set; }

        [Display(Name = "Number of cities")]
        public int NumberCities => Cities == null ? 0 : Cities.Count;

        public Guid FlagImageId { get; set; }

        //TODO: get another 30 days on azure and get blob storage link
        
        [Display(Name = "Flag")]
        public string ImageFullPath => FlagImageId == Guid.Empty
            ? "~/images/noimage.jpg"
        : $"https://brisa.blob.core.windows.net/countries/{FlagImageId}";
    }
}
