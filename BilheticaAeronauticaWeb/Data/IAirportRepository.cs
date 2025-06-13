using BilheticaAeronauticaWeb.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BilheticaAeronauticaWeb.Data
{
    public interface IAirportRepository :IGenericRepository<Airport>
    {
        IEnumerable<SelectListItem> GetComboAirports();
    }
}
