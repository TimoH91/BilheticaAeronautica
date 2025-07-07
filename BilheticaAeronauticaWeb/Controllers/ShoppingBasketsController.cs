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
            var shoppingBasketWithUser = new ShoppingBasketWithUserViewModel();

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

                if (user != null)
                {
                    var shoppingBasketTickets = await _shoppingBasketRepository.GetShoppingBasketTicketsAsync(user);

                    shoppingBasketWithUser.ShoppingBasketTickets = shoppingBasketTickets;

                    return View("Views/ShoppingBaskets/Index.cshtml", shoppingBasketWithUser);
                }

            }

            var shoppingBasketTickets2 = _basketHelper.GetBasketTickets(HttpContext.Session);

            shoppingBasketWithUser.ShoppingBasketTickets = shoppingBasketTickets2;

            return View("Views/ShoppingBaskets/Index.cshtml", shoppingBasketWithUser);
        }

        ////GET: ShoppingBaskets/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{

        //    return View();
        //}

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

            //var ticket = await _ticketRepository.GetTicketBySeatIdAsync(shoppingBasketTicket.SeatId, shoppingBasketTicket.Name, shoppingBasketTicket.Surname);

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
                var shoppingBasketTickets = new List<ShoppingBasketTicket>();

                var flight = await _flightRepository.GetByIdTrackedAsync(model.FlightId.Value);  
                var seat = await _seatRepository.GetByIdTrackedAsync(model.SeatId.Value);


                var basketTicket = _converterHelper.ToShoppingBasketTicket(model, true, flight, seat);
                User? user = null;

                if (User.Identity.IsAuthenticated)
                {
                    user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
                    basketTicket.User = user;

                    await _shoppingBasketRepository.AddShoppingBasketTicket(basketTicket);
                    shoppingBasketTickets = await _shoppingBasketRepository.GetShoppingBasketTicketsAsync(user);
                }
                else
                {
                    await _shoppingBasketRepository.AddShoppingBasketTicket(basketTicket);

                    shoppingBasketTickets = _basketHelper.GetBasketTickets(HttpContext.Session);
                    shoppingBasketTickets.Add(basketTicket);
                    _basketHelper.SaveBasketTickets(HttpContext.Session, shoppingBasketTickets);
                }

                if (basketTicket.Seat != null)
                {
                    await _ticketService.HoldSeat(basketTicket.Seat);
                }

                var shoppingBasketWithUserViewModel = new ShoppingBasketWithUserViewModel
                {
                    ShoppingBasketTickets = shoppingBasketTickets,
                    NewUser = user == null ? new RegisterNewUserViewModel() : null
                };

                return RedirectToAction("Index");
            }

            return RedirectToAction("Create", "Tickets");
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
            }

            var shoppingBasketWithUserViewModel = new ShoppingBasketWithUserViewModel
            {
                ShoppingBasketTickets = shoppingBasketTickets,
                NewUser = user == null ? new RegisterNewUserViewModel() : null
            };

            if (!shoppingBasketTickets.Any())
            {
                return View("NoShoppingBasket");
            }

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


    }
}



    

