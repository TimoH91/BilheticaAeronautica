using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Entities;
using BilheticaAeronauticaWeb.Helpers;
using BilheticaAeronauticaWeb.Models;
using BilheticaAeronauticaWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BilheticaAeronauticaWeb.Controllers
{
    public class ShoppingBasketsController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IShoppingBasketRepository _shoppingBasketRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IBasketHelper _basketHelper;
        private readonly ITicketService _ticketService;
        private readonly IFlightRepository _flightRepository;
        private readonly ISeatRepository _seatRepository;

        public ShoppingBasketsController(IUserHelper userHelper,
            IConverterHelper converterHelper,
            IShoppingBasketRepository shoppingBasketRepository,
            ITicketRepository ticketRepository,
            IBasketHelper basketHelper,
            ITicketService ticketService,
            IFlightRepository flightRepository,
            ISeatRepository seatRepository)
        {
            _userHelper = userHelper;
            _converterHelper = converterHelper;
            _shoppingBasketRepository = shoppingBasketRepository;
            _ticketRepository = ticketRepository;
            _basketHelper = basketHelper;
            _ticketService = ticketService;
            _flightRepository = flightRepository;
            _seatRepository = seatRepository;
        }

        // GET: ShoppingBaskets
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

                if (user != null)
                {
                    var shoppingBasketTicketsWithUser = await GetRegisteredUserBasketTickets(user);

                    return View("Views/ShoppingBaskets/Index.cshtml", shoppingBasketTicketsWithUser);
                }
            }

            var shoppingBasketTicketsWithoutUser = await GetUnegisteredUserBasketTickets();

            return View("Views/ShoppingBaskets/Index.cshtml", shoppingBasketTicketsWithoutUser);
        }



        ////GET: ShoppingBaskets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("TicketNotFound");
            }

            ShoppingBasketTicket shoppingBasketTicket = await _shoppingBasketRepository.GetShoppingBasketTicketAsync(id.Value);

            if (shoppingBasketTicket == null)
            {
                return new NotFoundViewResult("TicketNotFound");
            }

            return View(shoppingBasketTicket);
        }

        // GET: ShoppingBaskets/Create
        public IActionResult Create()
        {
            return View();
        }

        //// POST: ShoppingBaskets/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateShoppingBasketTicket(TicketViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Type == PassengerType.Infant && model.ResponsibleAdultTicketId == null)
                {
                    ViewBag.Adults = await GetAdultsFromBasket(model);

                    if (ViewBag.Adults.Count == 0)
                    {
                        return View("AddAdults");
                    }
                    else
                    {
                        return View("ChooseResponsibleForInfant", model);
                    }
                }
                else if(model.Type == PassengerType.Infant && model.ResponsibleAdultTicketId != null)
                {
                    await AssignResponsibleAdult(model);
                }
                
                if (model.FlightId != null)
                {
                        var basketTicket = _converterHelper.ToShoppingBasketTicket(model, true);

                        if (User.Identity.IsAuthenticated)
                        {
                            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

                            if (user != null)
                            {
                                await AddBasketTicketWithUser(user, basketTicket);
                            }
                        }
                        else
                        {
                            await AddBasketTicketWithoutUser(basketTicket);
                        }

                        if (basketTicket.SeatId != null)
                        {
                            await _ticketService.HoldSeat(basketTicket.SeatId.Value);
                        }

                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index");
        }



        // POST: ShoppingBaskets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Purchase()
        {
            var shoppingBasketTickets = new List<ShoppingBasketTicket>();
            User? user = null;

            if (User.Identity.IsAuthenticated)
            {
                user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
                shoppingBasketTickets = await _shoppingBasketRepository.GetShoppingBasketTicketsAsync(user);
            }
            else
            {
                shoppingBasketTickets = _basketHelper.GetBasketTickets(HttpContext.Session);

                foreach (var ticket in shoppingBasketTickets)
                {
                    ticket.Flight = await _flightRepository.GetByIdAsync(ticket.FlightId);
                }
            }

            if (!shoppingBasketTickets.Any())
            {
                return View("NoShoppingBasket");
            }

            var shoppingBasketWithUserViewModel = ConvertToTicketWithUserViewModel(shoppingBasketTickets, user);

            ViewBag.Seats = await GetSeatsViewBag(shoppingBasketWithUserViewModel);

            return View(shoppingBasketWithUserViewModel);

        }

        // GET: ShoppingBasket/Delete/5
        //[Authorize(Roles = "Customer")]
        public async Task<IActionResult> Delete(int? id)
        {
            User? user = null;

            if (id == null)
            {
                return new NotFoundViewResult("TicketNotFound");
            }

            var ticket = await _shoppingBasketRepository.GetShoppingBasketTicketAsync(id.Value);


            if (ticket == null)
            {
                return new NotFoundViewResult("TicketNotFound");
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _shoppingBasketRepository.GetShoppingBasketTicketAsync(id);

            try
            {
                await _shoppingBasketRepository.DeleteAsync(ticket);

                if (!User.Identity.IsAuthenticated)
                {
                    var basket = _basketHelper.GetBasketTickets(HttpContext.Session);
                    var localTickt = basket.FirstOrDefault(t => t.Id == id);

                    if (localTickt != null)
                    {
                        basket.Remove(localTickt);
                        _basketHelper.SaveBasketTickets(HttpContext.Session, basket);
                    }
                }

                if (ticket.Seat != null)
                {
                    await _ticketService.UnholdSeat(ticket.Seat);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {

                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    var errorModel = new ErrorViewModel
                    {
                        ErrorTitle = $"{ticket.Id} provavelmente está a ser usado!!",
                        ErrorMessage = $"{ticket.Id} não pode ser apagado.<br/><br/>" 
                    };

                    return View("Error", errorModel);
                }


                return View("Error", new ErrorViewModel
                {
                    ErrorTitle = "Erro de base de dados",
                    ErrorMessage = "Ocorreu um erro inesperado ao tentar apagar o ticket."
                });

            }
        }

        [HttpPost]
        [Route("ShoppingBaskets/GetAdultsFromBasket")]
        public async Task<List<SelectListItem>> GetAdultsFromBasket(TicketViewModel model)
        {
            var shoppingBasketTickets = new List<ShoppingBasketTicket>();

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

                if (user != null)
                {
                    shoppingBasketTickets = await _shoppingBasketRepository.GetShoppingBasketTicketsAsync(user);
                }
            }
            else
            {
                shoppingBasketTickets = _basketHelper.GetBasketTickets(HttpContext.Session);
            }

            var adults = _ticketService.FilterAdults(shoppingBasketTickets, model);

            var selectList = adults.Select(ticket => new SelectListItem
            {
                Value = ticket.Id.ToString(),
                Text = $"Name: {ticket.Name} {ticket.Surname}"
            }).ToList();

            return selectList;
        }

        public async Task AddBasketTicketWithUser(User user, ShoppingBasketTicket basketTicket)
        {
            user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
            basketTicket.User = user;

            await _shoppingBasketRepository.AddShoppingBasketTicket(basketTicket);
        }

        public async Task AddBasketTicketWithoutUser(ShoppingBasketTicket basketTicket)
        {
            await _shoppingBasketRepository.AddShoppingBasketTicket(basketTicket);
            var shoppingBasketTickets = _basketHelper.GetBasketTickets(HttpContext.Session);
            shoppingBasketTickets.Add(basketTicket);
            _basketHelper.SaveBasketTickets(HttpContext.Session, shoppingBasketTickets);
        }


        public async Task AssignResponsibleAdult(TicketViewModel model)
        {
            var responsibleAdultTicket = await _shoppingBasketRepository.GetShoppingBasketTicketAsync(model.ResponsibleAdultTicketId.Value);

            if (responsibleAdultTicket != null)
            {
                if (responsibleAdultTicket.Seat != null)
                {
                    model.SeatId = responsibleAdultTicket.SeatId;
                    await _ticketService.MakeResponsibleAdult(responsibleAdultTicket);
                    _basketHelper.UpdateTicket(HttpContext.Session, responsibleAdultTicket);
                }
            }
        }

        private async Task<ShoppingBasketWithUserViewModel> GetRegisteredUserBasketTickets(User user)
        {
            var shoppingBasketWithUser = new ShoppingBasketWithUserViewModel();

            var shoppingBasketTickets = await _shoppingBasketRepository.GetShoppingBasketTicketsAsync(user);

            var shoppingBasketTicketsViewModels = new List<ShoppingBasketTicketViewModel>();

            foreach (var ticket in shoppingBasketTickets)
            {
                shoppingBasketTicketsViewModels.Add(_converterHelper.ToShoppingBasketTicketViewModel(ticket));
            }

            shoppingBasketWithUser.ShoppingBasketTickets = shoppingBasketTicketsViewModels;

            return shoppingBasketWithUser;
        }

        private async Task<ShoppingBasketWithUserViewModel> GetUnegisteredUserBasketTickets()
        {
            var shoppingBasketWithNewUser = new ShoppingBasketWithUserViewModel();

            var shoppingBasketTickets = _basketHelper.GetBasketTickets(HttpContext.Session);

            foreach (var ticket in shoppingBasketTickets)
            {
                ticket.Flight = await _flightRepository.GetByIdAsync(ticket.FlightId);
            }

            var shoppingBasketTicketsViewModels = new List<ShoppingBasketTicketViewModel>();

            foreach (var ticket in shoppingBasketTickets)
            {
                shoppingBasketTicketsViewModels.Add(_converterHelper.ToShoppingBasketTicketViewModel(ticket));
            }

            shoppingBasketWithNewUser.ShoppingBasketTickets = shoppingBasketTicketsViewModels;

            shoppingBasketWithNewUser.ShoppingBasketTickets = shoppingBasketTicketsViewModels;

            return shoppingBasketWithNewUser;
        }

        private ShoppingBasketWithUserViewModel ConvertToTicketWithUserViewModel(List<ShoppingBasketTicket> basketTickets, User? user)
        {
            var shoppingBasketTicketsViewModels = new List<ShoppingBasketTicketViewModel>();

            foreach (var ticket in basketTickets)
            {
                shoppingBasketTicketsViewModels.Add(_converterHelper.ToShoppingBasketTicketViewModel(ticket));
            }

            var shoppingBasketWithUserViewModel = new ShoppingBasketWithUserViewModel
            {
                ShoppingBasketTickets = shoppingBasketTicketsViewModels,
                NewUser = user == null ? new RegisterNewUserViewModel() : null
            };

            return shoppingBasketWithUserViewModel;
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



    

