using System.Net.Sockets;
using BilheticaAeronauticaWeb.Entities;
using Microsoft.AspNetCore.Authentication;
using NuGet.ContentModel;

namespace BilheticaAeronauticaWeb.Services
{
    public interface IFlightService
    {
        Task<List<Seat>> CreateSeatsForFlightAsync(Flight flight);

        //ReattributeSeats(Flight flight);

        //RemoveSeatsAndTickets(List<Ticket> tickets, Flight flight);

        //MatchSeatsWithTickets(List<Ticket> tickets, List<Seat> newSeats);

        //MatchSeatsWithInfantTickets(List<Ticket> tickets);

        //RemoveSeatsFromTickets(List<Ticket> tickets);
        
    }
}
