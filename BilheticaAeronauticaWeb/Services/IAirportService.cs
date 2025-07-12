using BilheticaAeronauticaWeb.Entities;

namespace BilheticaAeronauticaWeb.Services
{
    public interface IAirportService
    {
        Task<bool> AllowAirportDeletionOrUpdate(Airport airport);
    }
}
