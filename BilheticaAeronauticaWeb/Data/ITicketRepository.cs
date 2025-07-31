using BilheticaAeronauticaWeb.Entities;
using Microsoft.EntityFrameworkCore;

namespace BilheticaAeronauticaWeb.Data
{
    public interface ITicketRepository : IGenericRepository<Ticket>
    {
        Task<Ticket> GetTicketBySeatIdAsync(int seatId, string firstName, string lastName);

        Task<List<Ticket>> GetTicketByOrderAsync(Order order);

        Task<List<Ticket>> GetTicketsByFlightIdAsync(int flightId);

        IQueryable GetAllWithUsers();

        Task<IEnumerable<Ticket>> GetFutureTickets(User user);

        Task<bool> IsAirportOnTicketAsync(int airportId);

        Task<Ticket> GetTicketsByUser(User user);

    }
}
