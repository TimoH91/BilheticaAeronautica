using System.Net.Sockets;
using BilheticaAeronauticaWeb.Entities;
using BilheticaAeronauticaWeb.Models;
using Microsoft.AspNetCore.Authentication;
using NuGet.ContentModel;

namespace BilheticaAeronauticaWeb.Services
{
    public interface IFlightService
    {
        Task<List<Seat>> CreateSeatsForFlightAsync(Flight flight);

        Task ReattributeSeats(Flight flight);

        Task AlterSeatsAndTickets(Flight flight);

        bool AllowDeletion(Flight flight);

        bool AllowEdit(FlightViewModel model);

    }
}
