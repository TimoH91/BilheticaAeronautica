using BilheticaAeronauticaWeb.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BilheticaAeronauticaWeb.Data
{
    public interface ISeatRepository : IGenericRepository<Seat>
    {
        IEnumerable<SelectListItem> GetComboSeats();

        Task<IEnumerable<Seat>> GetAvailableSeatsByFlight(int flightId);

        Task<IEnumerable<Seat>> GetAllSeatsByFlight(int flightId);

        Task<Seat> GetByIdTrackedAsync(int id);
    }
}
