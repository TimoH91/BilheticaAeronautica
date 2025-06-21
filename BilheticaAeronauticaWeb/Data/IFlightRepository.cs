using BilheticaAeronauticaWeb.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BilheticaAeronauticaWeb.Data
{
    public interface IFlightRepository : IGenericRepository<Flight>
    {
        IEnumerable<SelectListItem> GetComboFlights();

        Task<IEnumerable<object>> GetFlightsByOriginAndDestination(int originAirportId, int destinationAirport);
    }
}
