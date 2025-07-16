using BilheticaAeronauticaWeb.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BilheticaAeronauticaWeb.Data
{
    public interface IAirplaneRepository : IGenericRepository<Airplane>
    {
        IEnumerable<SelectListItem> GetComboAirplanes();

        Task<IEnumerable<Airplane>> GetAvailableAirplanes(Flight flight);
    }
}
