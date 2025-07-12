using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Entities;

namespace BilheticaAeronauticaWeb.Services
{
    public class AirportService : IAirportService
    {
        private readonly IFlightRepository _flightRepository;

        public AirportService(IFlightRepository flightRepository)
        {
            _flightRepository = flightRepository;
        }


        public async Task<bool> AllowAirportDeletionOrUpdate(Airport airport)
        {
            var isUsed = await _flightRepository.IsAirportUsedInAnyFlightAsync(airport.Id);

            if (isUsed)
            {
                return false;
            }

            return true;
        }
    }
}
