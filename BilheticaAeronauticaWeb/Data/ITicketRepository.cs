using BilheticaAeronauticaWeb.Entities;

namespace BilheticaAeronauticaWeb.Data
{
    public interface ITicketRepository : IGenericRepository<Ticket>
    {
        Task<Ticket> GetTicketBySeatIdAsync(int seatId, string firstName, string lastName);
    }
}
