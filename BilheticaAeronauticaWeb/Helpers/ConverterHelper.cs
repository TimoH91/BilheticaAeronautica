using BilheticaAeronauticaWeb.Entities;
using BilheticaAeronauticaWeb.Models;

namespace BilheticaAeronauticaWeb.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        public Country ToCountry(CountryViewModel model, Guid ImageId, bool isNew)
        {
            return new Country
            {
                Id = isNew ? 0 : model.Id,
                Name = model.Name,
                FlagImageId = ImageId
            };
        }

        public CountryViewModel ToCountryViewModel(Country country)
        {
            return new CountryViewModel
            {
                Id = country.Id,
                Name = country.Name,
                FlagImageId = country.FlagImageId, 
            };
        }
    }
}
