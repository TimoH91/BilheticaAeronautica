using BilheticaAeronauticaWeb.Entities;
using BilheticaAeronauticaWeb.Models;

namespace BilheticaAeronauticaWeb.Services
{
    public interface IAirplaneService
    {
        Task ReassignSeats(Airplane oldAirplane, Airplane editedAirplane);

        Task<bool> AllowAirplaneDeletion(Airplane airplane);

        Task<bool> AllowAirplaneStatusChange(Airplane airplane, Airplane editedAirplane);
    }
}
