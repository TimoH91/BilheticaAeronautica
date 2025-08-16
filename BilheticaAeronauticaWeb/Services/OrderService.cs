using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Entities;
using BilheticaAeronauticaWeb.Helpers;
using BilheticaAeronauticaWeb.Models;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Services
{
    public class OrderService : IOrderService
    {
        private readonly IShoppingBasketRepository _shoppingBasketRepository;
        private readonly IFlightRepository _flightRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly ITicketService _ticketService;
        private readonly ITicketRepository _ticketRepository;
        public OrderService(IShoppingBasketRepository shoppingBasketRepository, DataContext context, IFlightRepository flightRepository
            , IConverterHelper converterHelper, ITicketService ticketService, ITicketRepository ticketRepository    )
        {
            _shoppingBasketRepository = shoppingBasketRepository;
            _flightRepository = flightRepository;
            _converterHelper = converterHelper;
            _ticketService = ticketService;
            _ticketRepository = ticketRepository;
        }

        public async Task ClearShoppingBasket(List<ShoppingBasketTicket> shoppingBasketTickets)
        {
            foreach (var basketTicket in shoppingBasketTickets)
            {
                basketTicket.Flight = null;
                basketTicket.Seat = null;
                await _shoppingBasketRepository.DeleteAsync(basketTicket);
            }
            //await _shoppingBasketRepository.DeleteAsync(shoppingBasket);
        }

        public async Task ClearShoppingBasketByUser(User user)
        {
            var basketTickets = await _shoppingBasketRepository.GetShoppingBasketTicketsAsync(user);

            foreach (var basketTicket in basketTickets)
            {
                //basketTicket.Flight = null;
                //basketTicket.Seat = null;
                await _shoppingBasketRepository.DeleteAsync(basketTicket);
            }

        }

        public Order ConvertToOrder(List<ShoppingBasketTicket> basketTickets, User user)
        {
            Order order = new Order();

            order.TotalPrice = TotalPrice(basketTickets);
            order.User = user;
            order.OrderDate = DateTime.Now;

            return order;
        }

        public async Task<List<Ticket>> ConvertAdultAndChildTickets(List<ShoppingBasketTicket> shoppingBasketTickets, User user)
        {
            var tickets = new List<Ticket>();

            foreach (ShoppingBasketTicket basketTicket in shoppingBasketTickets)
            {
                if (basketTicket.PassengerType == PassengerType.Adult || basketTicket.PassengerType == PassengerType.Child)
                {
                    var flight = await _flightRepository.GetByIdTrackedAsync(basketTicket.FlightId);
                    var ticket = _converterHelper.BasketToTicket(basketTicket, flight);
                    ticket.UserId = user.Id;
                    tickets.Add(ticket);
                }
            }
            return tickets;
        }

        public async Task<List<Ticket>> AddTicketsAsync(List<Ticket> tickets, Order order)
        {
            foreach (Ticket ticket in tickets)
            {
                if (ticket is AdultTicket || ticket is ChildTicket)
                {
                    ticket.Order = order;
                    await _ticketService.OccupySeats(ticket.SeatId.Value);
                    await _ticketRepository.CreateAsync(ticket);
                }
                else
                {
                    ticket.Order = order;
                    await _ticketRepository.CreateAsync(ticket);
                }
            }

            return tickets;
        }

        private decimal TotalPrice(List<ShoppingBasketTicket> tickets)
        {
            decimal total = 0;

            foreach (var ticket in tickets)
            {
                total += ticket.Price;
            }

            return total;
        }

        public async Task<List<Ticket>> ConvertInfantTickets(List<Ticket> tickets, List<ShoppingBasketTicket> shoppingBasketTickets, User user)
        {
            var infantTickets = new List<Ticket>();

            foreach (ShoppingBasketTicket basketTicket in shoppingBasketTickets)
            {
                if (basketTicket.PassengerType == PassengerType.Infant)
                {
                    foreach (var ticket in tickets.OfType<AdultTicket>())
                    {
                        if (basketTicket.SeatId == ticket.SeatId)
                        {
                            basketTicket.ResponsibleAdultTicketId = AssignResponsibleAdults(ticket.Id);
                            var flight = await _flightRepository.GetByIdTrackedAsync(basketTicket.FlightId);
                            var infantTicket = _converterHelper.BasketToTicket(basketTicket, flight);
                            infantTicket.UserId = user.Id;
                            infantTickets.Add(infantTicket);
                        }
                    }
                }
            }

            return infantTickets;
        }

        private int AssignResponsibleAdults(int id)
        {
            return id;
        }

    }
}
