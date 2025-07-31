using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Entities;
using BilheticaAeronauticaWeb.Models;

namespace BilheticaAeronauticaWeb.Services
{
    public class FlightService : IFlightService
    {
        private readonly IAirplaneRepository _airplaneRepository;
        private readonly ISeatRepository _seatRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IShoppingBasketRepository _shoppingBasketRepository;

        public FlightService(IAirplaneRepository airplaneRepository,
            ISeatRepository seatRepository,
            ITicketRepository ticketRepository,
            IShoppingBasketRepository shoppingBasketRepository)
        {
            _airplaneRepository = airplaneRepository;
            _seatRepository = seatRepository;
            _ticketRepository = ticketRepository;
            _shoppingBasketRepository = shoppingBasketRepository;
        }


        public async Task ReattributeSeats(Flight flight)
        {
            var tickets = await RemoveSeatsFromTickets(flight);

            var newSeats = await CreateSeatsForFlightAsync(flight);

            MatchSeatsWithTickets(tickets, newSeats);

            await AssignInfantSeats(tickets);

            RemoveFlightFromSeatlessTickets(tickets);

            await UpdateRangeTickets(tickets);
        }

        public async Task<List<Seat>> CreateSeatsForFlightAsync(Flight flight)
        {
            var airplane = await _airplaneRepository.GetByIdAsync(flight.AirplaneId);

            if (airplane == null)
            {
                throw new Exception("Airplane not found.");
            }

            var seats = new List<Seat>();

            for (int row = 1; row <= airplane.Rows; row++)
            {
                for (int col = 1; col <= airplane.SeatsPerRow; col++)
                {
                    seats.Add(new Seat
                    {
                        Flight = flight,
                        Row = row,
                        Column = col,
                        Occupied = false
                    });
                }
            }

            await _seatRepository.CreateRangeAsync(seats);

            return seats;
        }


        private async Task<List<Ticket>>RemoveSeatsFromTickets(Flight flight)
        {
            var tickets = await _ticketRepository.GetTicketsByFlightIdAsync(flight.Id);
            var shoppingBasketTickets = await _shoppingBasketRepository.GetTicketsByFlightIdAsync(flight.Id);

            foreach (Ticket ticket in tickets)
            {
                ticket.Seat = null;
                ticket.SeatId = null;
            }
            foreach (ShoppingBasketTicket basketTicket in shoppingBasketTickets)
            {
                basketTicket.Seat = null;
                basketTicket.SeatId = null;
            }

            await _shoppingBasketRepository.UpdateRangeAsync(shoppingBasketTickets);
            await _ticketRepository.UpdateRangeAsync(tickets); 

            var seats = await _seatRepository.GetAllSeatsByFlight(flight.Id);

            await _seatRepository.DeleteRangeAsync(seats);



            return tickets;
        }

        private void MatchSeatsWithTickets(List<Ticket> tickets, List<Seat> newSeats)
        {
            if (tickets.Count > 0)
            {
                if (tickets.Count >= newSeats.Count)
                {
                    for (int i = 0, j = 0; j < newSeats.Count;)
                    {
                        if (tickets[i].Type != PassengerType.Infant)
                        {
                            tickets[i].Seat = newSeats[j];
                            newSeats[j].Occupied = true;
                            i++;
                            j++;
                        }
                        else
                        {
                            i++;
                        }
                    }
                }
                else if (newSeats.Count > tickets.Count)
                {
                    for (int i = 0, j = 0; i < tickets.Count;)
                    {
                        if (tickets[i].Type != PassengerType.Infant)
                        {
                            tickets[i].Seat = newSeats[j];
                            newSeats[j].Occupied = true;
                            i++;
                            j++;
                        }
                        else
                        {
                            i++;
                        }
                    }
                }
            }
        }

        private async Task AssignInfantSeats(List<Ticket> tickets)
        {
            foreach (var ticket in tickets.OfType<InfantTicket>())
            {
                    var responsibleAdult = tickets.FirstOrDefault(r => r.Id == ticket.ResponsibleAdultTicketId);

                    if (responsibleAdult != null)
                    {
                        ticket.Seat = responsibleAdult.Seat;
                    }
            }
        }

        private void RemoveFlightFromSeatlessTickets(List<Ticket> tickets)
        {
            var ticketsWithoutSeat = tickets.Where(t => t.Seat == null).ToList();

            foreach (Ticket ticket in ticketsWithoutSeat)
            {
                ticket.Flight = null;
            }
        }

        private async Task UpdateRangeTickets(List<Ticket> tickets)
        {
            foreach (var ticket in tickets)
            {
                await _ticketRepository.UpdateAsync(ticket);
            }
        }

        public async Task AlterSeatsAndTickets(Flight flight)
        {
            var seats = await _seatRepository.GetAllSeatsByFlight(flight.Id);
            var tickets = await _ticketRepository.GetTicketsByFlightIdAsync(flight.Id);
            var shoppingBasketTickets = await _shoppingBasketRepository.GetTicketsByFlightIdAsync(flight.Id);

            foreach (var ticket in tickets)
            {
                ticket.FlightId = null;
                ticket.Flight = null;
                ticket.SeatId = null;
                ticket.Seat = null;
                await _ticketRepository.UpdateAsync(ticket);
            }

            await _shoppingBasketRepository.DeleteRangeAsync(shoppingBasketTickets);

            await _seatRepository.DeleteRangeAsync(seats);
        }


        public bool AllowDeletion(Flight flight)
        {          
            if (flight.Date < DateTime.Now.Date || (flight.Date == DateTime.Now.Date && flight.Time < DateTime.Now.TimeOfDay))
            {
                return false;
            }

            return true;
        }

        public bool AllowEdit(FlightViewModel model)
        {
            if (model.Date < DateTime.Now.Date || (model.Date == DateTime.Now.Date && model.Time < DateTime.Now.TimeOfDay))
            {
                return false;
            }

            return true;
        }
    }
}
