using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Entities;
using BilheticaAeronauticaWeb.Helpers;
using BilheticaAeronauticaWeb.Models;
using Microsoft.AspNetCore.Http;


namespace BilheticaAeronauticaWeb.Services
{
    public class TicketService : ITicketService
    {
        private readonly ISeatRepository _seatRepository;
        private readonly IShoppingBasketRepository _shoppingBasketRepository;
        private readonly IBasketHelper _basketHelper;
        private readonly IFlightRepository _flightRepository;


        public TicketService(ISeatRepository seatRepository,
            IShoppingBasketRepository shoppingBasketRepository,
            IBasketHelper basketHelper,
            IFlightRepository flightRepository)
        {
            _seatRepository = seatRepository;
            _shoppingBasketRepository = shoppingBasketRepository;
            _basketHelper = basketHelper;
            _flightRepository = flightRepository;
        }


        public async Task OccupySeats(int seatId)
        {
                var seat = await _seatRepository.GetByIdAsync(seatId);
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

        public List<ShoppingBasketTicket> FilterAdults(List<ShoppingBasketTicket> tickets, TicketViewModel model)
        {
            return tickets
                .Where(ticket => ticket.PassengerType == PassengerType.Adult && ticket.Flight.Id == model.FlightId && ticket.IsResponsibleAdult == false)
                .ToList();
        }

        public async Task MakeResponsibleAdult(ShoppingBasketTicket ticket)
        {
            ticket.IsResponsibleAdult = true;
            await _shoppingBasketRepository.UpdateAsync(ticket); 
        }


        public async Task<bool> AllowTicketChanges(TicketViewModel model)
        {
            if (model.FlightId != null)
            {
                var existingFlight = await _flightRepository.GetByIdAsync(model.FlightId.Value);

                if (existingFlight != null)
                {
                    if (existingFlight.Date > DateTime.Now.Date || existingFlight.Date == DateTime.Now.Date && existingFlight.Time > DateTime.Now.TimeOfDay)
                    {
                        return true;
                    }
                }
            }
           
            return false;
        }


    }
    
}
