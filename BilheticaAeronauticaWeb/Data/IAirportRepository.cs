using BilheticaAeronauticaWeb.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BilheticaAeronauticaWeb.Data
{
    public interface IAirportRepository :IGenericRepository<Airport>
    {
        IEnumerable<SelectListItem> GetComboAirports();

        Task<Airport> GetByIdTrackedAsync(int id);
    }
}
