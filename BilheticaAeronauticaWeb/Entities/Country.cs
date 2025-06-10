using System.ComponentModel.DataAnnotations;

namespace BilheticaAeronauticaWeb.Entities
{
    public class Country : IEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The field {0} can contain {1} characters.")]
        public string Name { get; set; }

        public ICollection<City> Cities { get; set; }

        [Display(Name = "Number of cities")]
        public int NumberCities => Cities == null ? 0 : Cities.Count;

        public Guid FlagImageId { get; set; }

        //TODO: get another 30 days on azure and get blob storage link

        public string FlagImageFullPath => FlagImageId == Guid.Empty
            ? "~/images/noimage.jpg"
        : $"~/images/flags/{FlagImageId}.jpg";
    }
}
