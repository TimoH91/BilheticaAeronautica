using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Entities;

namespace BilheticaAeronauticaWeb.Services
{
    public class AirportService : IAirportService
    {
        private readonly IFlightRepository _flightRepository;
        private readonly ITicketRepository _ticketRepository;

        public AirportService(IFlightRepository flightRepository, ITicketRepository ticketRepository)
        {
            _flightRepository = flightRepository;
            _ticketRepository = ticketRepository;
        }


        public async Task<bool> AllowAirportDeletionOrUpdate(Airport airport)
        {
            var isUsed = await _flightRepository.IsAirportUsedInAnyFlightAsync(airport.Id);
            var isOnTickets = await _ticketRepository.IsAirportOnTicketAsync(airport.Id);

            if (isUsed || isOnTickets)
            {
                return false;
            }

            return true;
        }
    }
}
