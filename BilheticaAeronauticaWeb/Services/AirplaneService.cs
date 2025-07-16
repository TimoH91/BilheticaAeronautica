using System.Threading.Tasks;
using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Entities;
using BilheticaAeronauticaWeb.Migrations;
using BilheticaAeronauticaWeb.Models;
using EFCore.BulkExtensions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using PassengerType = BilheticaAeronauticaWeb.Entities.PassengerType;


namespace BilheticaAeronauticaWeb.Services
{
    public class AirplaneService : IAirplaneService
    {
        private readonly IFlightRepository _flightRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly ISeatRepository _seatRepository;
        private readonly IShoppingBasketRepository _shoppingBasketRepository;
        private readonly IAirplaneRepository _airplaneRepository;

        public AirplaneService(IFlightRepository flightRepository,
            ITicketRepository ticketRepository,
            ISeatRepository seatRepository,
            IShoppingBasketRepository shoppingBasketRepository,
            IAirplaneRepository airplaneRepository)
        {
            _flightRepository = flightRepository;
            _ticketRepository = ticketRepository;
            _seatRepository = seatRepository;
            _shoppingBasketRepository = shoppingBasketRepository;
            _airplaneRepository = airplaneRepository;
        }

        public async Task ReassignSeats(Airplane oldAirplane, Airplane editedAirplane)
        {
            if (SeatConfigurationChanged(oldAirplane, editedAirplane))
            {
                var flights = await GetFutureFlightsWithAirplane(oldAirplane);

                foreach (var flight in flights)
                {
                    var tickets = await _ticketRepository.GetTicketsByFlightIdAsync(flight.Id);

                    var basketTickets = await _shoppingBasketRepository.GetTicketsByFlightIdAsync(flight.Id);

                    await RemoveSeatsFromTickets(tickets, basketTickets, flight);

                    var seats = await CreateNewSeats(editedAirplane, flight);

                    AssignNewSeatsToTickets(tickets, seats);

                    AssignInfantSeats(tickets);

                    RemoveFlightFromSeatlessTickets(tickets);

                    await UpdateRangeTickets(tickets);
                }

            }
        }

        private bool SeatConfigurationChanged(Airplane oldAirplane, Airplane editedAirplane)
        {
            return oldAirplane.Rows != editedAirplane.Rows || oldAirplane.SeatsPerRow != editedAirplane.SeatsPerRow;
        }

        private async Task<List<Flight>> GetFutureFlightsWithAirplane(Airplane airplane)
        {
            var flights = await _flightRepository.GetFlightsByAirplane(airplane.Id);

            var futureFlightsWithAirplane = new List<Flight>();

            foreach (Flight flight in flights)
            {
                if (flight.Date > DateTime.Now.Date)
                {
                    futureFlightsWithAirplane.Add(flight);
                }
                else if (flight.Date == DateTime.Now.Date && flight.Time > DateTime.Now.TimeOfDay)
                {
                    futureFlightsWithAirplane.Add(flight);
                }
            }
            return futureFlightsWithAirplane;
        }

        private async Task RemoveSeatsFromTickets(List<Ticket> tickets, List<ShoppingBasketTicket> basketTickets, Flight flight)
        {
                List<Seat> newSeats = new List<Seat>();

                var seatsWithFlight = await _seatRepository.GetAllSeatsByFlight(flight.Id);

                foreach (Ticket ticket in tickets)
                {
                    ticket.Seat = null;
                }
                foreach (var basketTicket in basketTickets)
                {
                    basketTicket.Seat = null;
                }

            await UpdateRangeTickets(tickets);
            await UpdateRangeShoppingBasketTickets(basketTickets);
            await _seatRepository.DeleteRangeAsync(seatsWithFlight);
        }

        private async Task<List<Seat>> CreateNewSeats(Airplane editedAirplane, Flight flight)
        {
            var seats = new List<Seat>();

            for (int row = 1; row <= editedAirplane.Rows; row++)
            {
                for (int col = 1; col <= editedAirplane.SeatsPerRow; col++)
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

        private void AssignNewSeatsToTickets(List<Ticket> tickets, List<Seat> newSeats)
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

        private void AssignInfantSeats(List<Ticket> tickets)
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

        private async Task UpdateRangeShoppingBasketTickets(List<ShoppingBasketTicket> basketTickets)
        {
            foreach (var ticket in basketTickets)
            {
                await _shoppingBasketRepository.UpdateAsync(ticket);
            }
        }

        public async Task<bool> AllowAirplaneDeletion(Airplane airplane)
        {
            bool canDelete = false;

            var flightsWithAirplane = await _flightRepository.GetFlightsByAirplane(airplane.Id);

            if (!flightsWithAirplane.Any())
            {
                canDelete = true;
            }

            return canDelete;
        }

        public async Task<bool> AllowAirplaneStatusChange(Airplane airplane, Airplane editedAirplane)
        {
            bool canEdit = true;

            if (airplane.Status != editedAirplane.Status)
            {
                var flightsWithAirplane = await _flightRepository.GetFlightsByAirplane(airplane.Id);

                var futureFlightsWithAirplane = new List<Flight>();

                foreach (Flight flight in flightsWithAirplane)
                {
                    if (flight.Date > DateTime.Now.Date)
                    {
                        futureFlightsWithAirplane.Add(flight);
                    }
                    else if (flight.Date == DateTime.Now.Date && flight.Time > DateTime.Now.TimeOfDay)
                    {
                        futureFlightsWithAirplane.Add(flight);
                    }
                }

                if (futureFlightsWithAirplane.Any())
                {
                   canEdit = false;
                }

            }

            return canEdit;
        }

    }
}
