using BilheticaAeronauticaWeb.Entities;
using BilheticaAeronauticaWeb.Models;

namespace BilheticaAeronauticaWeb.Helpers
{
    public interface IConverterHelper
    {
            Country ToCountry(CountryViewModel model, Guid ImageId, bool isNew);

            CountryViewModel ToCountryViewModel(Country country);

            Task<Airport> ToAirport(AirportViewModel model, bool isNew);

            AirportViewModel ToAirportViewModel(Airport airport);

            Airplane ToAirplane(AirplaneViewModel model, Guid ImageId, bool isNew);

            AirplaneViewModel ToAirplaneViewModel(Airplane airplane);

    }
}
