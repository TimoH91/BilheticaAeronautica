using BilheticaAeronauticaWeb.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BilheticaAeronauticaWeb.Data
{
    public interface IFlightRepository : IGenericRepository<Flight>
    {
        IEnumerable<SelectListItem> GetComboFlights();

        Task<IEnumerable<Flight>> GetFlightsByOriginAndDestination(int originAirportId, int destinationAirport);

        Task<IEnumerable<Flight>> GetFlightsByOriginDestinationAndDate(int originAirportId, int destinationAirport, DateTime date);

        //IQueryable<Flight> GetFlightsByOriginAndDestination(int originAirportId, int destinationAirportId);

        Task<Flight> GetByIdTrackedAsync(int id);

        Task<IEnumerable<Flight>> GetFlightsByAirplane(int airplaneId);

        Task<bool> IsAirportUsedInAnyFlightAsync(int airportId);

        IQueryable<Flight> GetAllFutureFlights();
    }
}
