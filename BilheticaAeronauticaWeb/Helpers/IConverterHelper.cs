using BilheticaAeronauticaWeb.Entities;
using BilheticaAeronauticaWeb.Models;

namespace BilheticaAeronauticaWeb.Helpers
{
    public interface IConverterHelper
    {
            Country ToCountry(CountryViewModel model, Guid ImageId, bool isNew);

            CountryViewModel ToCountryViewModel(Country country);
        
    }
}
