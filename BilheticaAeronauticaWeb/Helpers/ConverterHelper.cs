using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Entities;
using BilheticaAeronauticaWeb.Models;

namespace BilheticaAeronauticaWeb.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        private readonly ICountryRepository _countryRepository;


        public ConverterHelper(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

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

        public async Task<Airport> ToAirport(AirportViewModel model, bool isNew)
        {
            return new Airport
            {
                Id = isNew ? 0 : model.Id,
                Name = model.Name,
                City = await _countryRepository.GetCityAsync(model.CityId),
                Country = await _countryRepository.GetCountryAsync(model.CountryId)
            };
        }

        public AirportViewModel ToAirportViewModel(Airport airport)
        {
            return new AirportViewModel
            {
                Id = airport.Id,
                Name = airport.Name,
                CityId = airport.City.Id,
                CountryId = airport.Country.Id
            };
        }

        public Airplane ToAirplane(AirplaneViewModel model, Guid ImageId, bool isNew)
        {
            return new Airplane
            {
                Id = isNew ? 0 : model.Id,
                Name = model.Name,
                Manufacturer = model.Manufacturer,
                Rows = model.Rows,
                SeatsPerRow = model.SeatsPerRow,
                ImageId = ImageId,
                Status = model.Status
            };
        }

        public AirplaneViewModel ToAirplaneViewModel(Airplane airplane)
        {
            return new AirplaneViewModel
            {
                Id = airplane.Id,
                Name = airplane.Name,
                Manufacturer = airplane.Manufacturer,
                Rows = airplane.Rows,
                SeatsPerRow = airplane.SeatsPerRow,
                ImageId = airplane.ImageId,
                Status = airplane.Status
            };
        }
    }
}
