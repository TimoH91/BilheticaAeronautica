using BilheticaAeronauticaWeb.Entities;
using BilheticaAeronauticaWeb.Models;

namespace BilheticaAeronauticaWeb.Services
{
    public interface ITicketService
    {
        //Task OccupySeats(List<Ticket> Tickets);

        //Task UnoccupySeats(List<Ticket> Tickets);

        List<ShoppingBasketTicket> FilterAdults(List<ShoppingBasketTicket> tickets, TicketViewModel ticket);
        Task OccupySeats(int seatId);

        Task UnoccupySeats(Seat seat);
        Task<IEnumerable<Seat>> UnholdSeats(IEnumerable<Seat> seats);

        Task UnholdSeat(Seat seat);

        List<Seat> RemoveHeldSeats(IEnumerable<Seat> seats);

        Task HoldSeat(Seat seat);

        Task MakeResponsibleAdult(ShoppingBasketTicket ticket);

        Task<bool> AllowTicketChanges(TicketViewModel model);

    }
}
