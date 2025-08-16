using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Entities;
using BilheticaAeronauticaWeb.Models;
using BilheticaAeronauticaWeb.Helpers;
using System.Diagnostics;
using NuGet.Packaging;
using BilheticaAeronauticaWeb.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace BilheticaAeronauticaWeb.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderService _orderService;
        private readonly IConverterHelper _converterHelper;
        private readonly IShoppingBasketRepository _shoppingBasketRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly ISeatRepository _seatRepository;
        private readonly IFlightRepository _flightRepository;
        private readonly IAirportRepository _airportRepository;
        private readonly IUserHelper _userHelper;
        private readonly IBasketHelper _basketHelper;
        private readonly IMailHelper _mailHelper;
        private readonly ITicketService _ticketService;

        public OrdersController(IOrderRepository orderRepository, IConverterHelper converterHelper,
            IShoppingBasketRepository shoppingBasketRepository, ITicketRepository ticketRepository, IUserHelper userHelper,
            IOrderService orderService, IBasketHelper basketHelper, IMailHelper mailHelper, ITicketService ticketService,
            ISeatRepository seatRepository, IAirportRepository airportRepository, IFlightRepository flightRepository)
        {
            _orderRepository = orderRepository;
            _converterHelper = converterHelper;
            _shoppingBasketRepository = shoppingBasketRepository;
            _ticketRepository = ticketRepository;
            _userHelper = userHelper;
            _orderService = orderService;
            _basketHelper = basketHelper;
            _mailHelper = mailHelper;
            _ticketService = ticketService;
            _seatRepository = seatRepository;
            _airportRepository = airportRepository;
            _flightRepository = flightRepository;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var orders = await _orderRepository.GetOrdersByUserAsync(User.Identity.Name);

                if (orders == null)
                {
                    return new NotFoundViewResult("NoBookings");
                }

                return View(orders);
            }

            return new NotFoundViewResult("NoBookings");

        }

        // GET: Orders
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> CustomerOrders()
        {
            if (User.Identity.IsAuthenticated)
            {
                var orders = _orderRepository.GetAll();

                if (orders == null)
                {
                    return new NotFoundViewResult("NoBookings");
                }

                return View(orders);
            }

            return new NotFoundViewResult("NoBookings");

        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("OrderNotFound");
            }

            var order = await _orderRepository.GetByIdAsync(id.Value);

            if (id == null)
            {
                return new NotFoundViewResult("OrderNotFound");
            }

            return View(order);
        }


        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Purchase(ShoppingBasketWithUserViewModel model)
        {

            foreach (var basketTicket in model.ShoppingBasketTickets)
            {
                if (basketTicket.SeatId == null)
                {
                    ViewBag.Seats = await GetSeatsViewBag(model);
                    return View("~/Views/ShoppingBaskets/Purchase.cshtml", model);
                }
            }

            var user = await GetOrderUser(model);

            if (user == null)
            {
                return new NotFoundViewResult("UserNotFound");
            }

            var shoppingBasketTickets = ConvertToShoppingBasketTickets(model);

            var order = ConvertToOrder(shoppingBasketTickets, user);

            await _orderRepository.CreateAsync(order);

            var tickets = await ConvertAdultAndChildTickets(shoppingBasketTickets, user);

            tickets = await AddTicketsAsync(tickets, order);

            var infantTickets = await ConvertInfantTickets(tickets, shoppingBasketTickets, user);

            infantTickets = await AddTicketsAsync(infantTickets, order);

            await ClearBasket(user, model);

            return View("ThankyouForBooking");
        }

        // GET: Orders/Edit/5
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("OrderNotFound");
            }

            var order = await _orderRepository.GetByIdAsync(id.Value);

            if (order == null)
            {
                return new NotFoundViewResult("OrderNotFound");
            }

            var model = _converterHelper.ToOrderViewModel(order);

            return View(model);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(OrderViewModel model)
        {

            User user = null;

            if (User.Identity.IsAuthenticated)
            { 
                user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
            }

            if(ModelState.IsValid && user != null)
            { 
                try
                {
                    var order = _converterHelper.ToOrder(model, false, user);

                    await _orderRepository.UpdateAsync(order);
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!await _orderRepository.ExistAsync(model.Id))
                    {
                        return new NotFoundViewResult("OrderNotFound"); ;
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Orders/Delete/5
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("OrderNotFound");
            }

            var ticket = await _orderRepository.GetByIdAsync(id.Value);

            if (ticket == null)
            {
                return new NotFoundViewResult("OrderNotFound");
            }

            return View(ticket);
        }

        // POST: Orders/Delete/5
        [Authorize(Roles = "Staff")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);

            try
            {
                await _orderRepository.DeleteAsync(order);

                return RedirectToAction(nameof(Index));
            }

            catch (DbUpdateException ex)
            {

                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    var errorModel = new ErrorViewModel
                    {
                        ErrorTitle = $"{order.Id} provavelmente está a ser usado!!",
                        ErrorMessage = $"{order.Id} não pode ser apagado<br/><br/>"
                            
                    };

                    return View("Error", errorModel);
                }


                return View("Error", new ErrorViewModel
                {
                    ErrorTitle = "Erro de base de dados",
                    ErrorMessage = "Ocorreu um erro inesperado ao tentar apagar o aeroporto."
                });

            }
        }

        private async Task<User> RegisterNewUser(RegisterNewUserViewModel newUser)
        {
            User user = new User
            {
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Email = newUser.Username,
                UserName = newUser.Username,
                Role = "Customer"
            };

            var result = await _userHelper.AddUserAsync(user, newUser.Password);

            if (result != IdentityResult.Success)
            {
                ModelState.AddModelError(string.Empty, "The user couldn't be created.");
                //return View(model);
            }
            else
            {
                await _userHelper.AddUserToRoleAsync(user, "Customer");
            }

            string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
            string tokenLink = Url.Action("ConfirmEmail", "Account", new
            {
                userid = user.Id,
                token = myToken
            }, protocol: HttpContext.Request.Scheme);

            Response response = _mailHelper.SendEmail(newUser.Username, "Email confirmation", $"<h1>Email Confirmation</h1>" +
                                                                     $"To allow the user, " +
                                                                    $"please click in this link:</br></br><a href = \"{tokenLink}\">Confirm Email</a>");


            if (response.IsSuccess)
            {

                ViewBag.Message = "The instructions to allow you user has been sent to email";
                //return View(model);
            }


            ModelState.AddModelError(string.Empty, "The user couldn't be logged.");

            return user;
        }

        private List<ShoppingBasketTicket> ConvertToShoppingBasketTickets(ShoppingBasketWithUserViewModel model)
        {
            var shoppingBasketTickets = new List<ShoppingBasketTicket>();

            foreach (var ticket in model.ShoppingBasketTickets)
            {
                shoppingBasketTickets.Add(_converterHelper.ToShoppingBasketTicketFromModel(ticket, false));
            }

            return shoppingBasketTickets;
        }

        private async Task<List<Ticket>> ConvertAdultAndChildTickets(List<ShoppingBasketTicket> shoppingBasketTickets, User user)
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

        private decimal TotalPrice(List<ShoppingBasketTicket> tickets)
        {
            decimal total = 0;

            foreach (var ticket in tickets)
            {
                total += ticket.Price;
            }

            return total;
        }

        private async Task<User> GetOrderUser(ShoppingBasketWithUserViewModel model)
        {
            User user;

            if (model.NewUser != null)
            {
                return user = await RegisterNewUser(model.NewUser);
            }
            else
            {
                return user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
            }
        }

        private Order ConvertToOrder(List<ShoppingBasketTicket> basketTickets, User user)
        {
            Order order = new Order();

            order.TotalPrice = TotalPrice(basketTickets);
            order.User = user;
            order.OrderDate = DateTime.Now;

            return order;
        }

        //private async Task<IActionResult>AddSeatsIfMissing(List<Ticket> tickets)
        //{

        //}
        private async Task<List<Ticket>> AddTicketsAsync(List<Ticket> tickets, Order order)
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

        private async Task<List<Ticket>> ConvertInfantTickets(List<Ticket> tickets, List<ShoppingBasketTicket> shoppingBasketTickets, User user)
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

        private async Task ClearBasket(User user, ShoppingBasketWithUserViewModel model)
        {
            if (model.NewUser == null)
            {
                await _orderService.ClearShoppingBasketByUser(user);
            }
            else
            {
                _basketHelper.ClearBasket(HttpContext.Session);
            }
        }

        private async Task<List<ShoppingBasketTicket>> GetShoppingBasketTickets(ShoppingBasketWithUserViewModel model)
        {
            var shoppingBasketTickets = new List<ShoppingBasketTicket>();


            if (User.Identity.IsAuthenticated)
            {
                var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

                return shoppingBasketTickets = await _shoppingBasketRepository.GetShoppingBasketTicketsAsync(user);
      
            }
            else
            {
                var sessionTickets = _basketHelper.GetBasketTickets(HttpContext.Session);
                var shopBasketTickets = new List<ShoppingBasketTicket>();

                foreach (var ticket in sessionTickets)
                {
                    shopBasketTickets.Add(await _shoppingBasketRepository.GetShoppingBasketTicketAsync(ticket.Flight.Id));
                }

                return shopBasketTickets;
            }
        }

        private async Task<List<SelectListItem>> GetSeatsViewBag(ShoppingBasketWithUserViewModel model)
        {
            var seats = await _seatRepository.GetAvailableSeatsByFlight(model.ShoppingBasketTickets[0].FlightId);

            var selectList = seats.Select(seat => new SelectListItem
            {
                Value = seat.Id.ToString(),
                Text = $"Row: {seat.Row} Number: {seat.Column}"
            }).ToList();

            return selectList;
        }
    }
}
