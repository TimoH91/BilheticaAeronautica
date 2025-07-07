using BilheticaAeronauticaWeb.Entities;

namespace BilheticaAeronauticaWeb.Services
{
    public interface ITicketService
    {
        //Task OccupySeats(List<Ticket> Tickets);

        //Task UnoccupySeats(List<Ticket> Tickets);


        Task OccupySeats(Seat seat);

        Task UnoccupySeats(Seat seat);
        Task<IEnumerable<Seat>> UnholdSeats(IEnumerable<Seat> seats);

        Task UnholdSeat(Seat seat);

        List<Seat> RemoveHeldSeats(IEnumerable<Seat> seats);

        Task HoldSeat(Seat seat);

    }
}
