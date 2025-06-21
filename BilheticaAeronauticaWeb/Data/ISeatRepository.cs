using BilheticaAeronauticaWeb.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BilheticaAeronauticaWeb.Data
{
    public interface ISeatRepository : IGenericRepository<Seat>
    {
        IEnumerable<SelectListItem> GetComboSeats();

        Task<IEnumerable<SelectListItem>> GetSeatsByFlight(int flightId);
    }
}
