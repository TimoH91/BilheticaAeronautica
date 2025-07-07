using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Entities;

namespace BilheticaAeronauticaWeb.Services
{
    public class TicketService : ITicketService
    {
        private readonly ISeatRepository _seatRepository;


        public TicketService(ISeatRepository seatRepository)
        {
            _seatRepository = seatRepository;
        }


        public async Task OccupySeats(Seat seat)
        {
                seat.Occupied = true;
                await _seatRepository.UpdateAsync(seat);
        }

        public async Task UnoccupySeats(Seat seat)
        {
                seat.Occupied = false;
                await _seatRepository.UpdateAsync(seat);
        }


        public async Task HoldSeat(Seat seat)
        {
                seat.IsHeld = true;
                seat.HoldingTime = DateTime.Now;
                await _seatRepository.UpdateAsync(seat);
        }

        public async Task<IEnumerable<Seat>> UnholdSeats(IEnumerable<Seat> seats)
        {
            if (seats != null && seats.Any()) 
            {
                foreach (Seat seat in seats)
                {
                    if (seat.IsHeld == true)
                    {
                        if (seat.HoldingTime < DateTime.Now.AddMinutes(-60))
                        {
                            seat.IsHeld = false;
                            seat.HoldingTime = null;
                            await _seatRepository.UpdateAsync(seat);

                        }
                    }
                }
            }
            return seats;
        }

        public List<Seat> RemoveHeldSeats(IEnumerable<Seat> seats)
        {
            if (seats != null && seats.Any())
            {
                var seatsList = seats.ToList();

                foreach (Seat seat in seats)
                {
                    if (seat.IsHeld == true)
                    {
                        seatsList.Remove(seat);
                    }
                }
                return seatsList;
            }
            return seats.ToList();
        }

        public async Task UnholdSeat(Seat seat)
        {
                seat.IsHeld = false;
                seat.HoldingTime = null;
                await _seatRepository.UpdateAsync(seat);
        }
    }
    
}
