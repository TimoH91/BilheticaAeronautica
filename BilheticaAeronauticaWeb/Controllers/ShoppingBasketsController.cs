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

namespace BilheticaAeronauticaWeb.Controllers
{
    public class ShoppingBasketsController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IShoppingBasketRepository _shoppingBasketRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IBasketHelper _basketHelper;

        public ShoppingBasketsController(IUserHelper userHelper, IConverterHelper converterHelper, IShoppingBasketRepository shoppingBasketRepository, ITicketRepository ticketRepository, IBasketHelper basketHelper)
        {
            _userHelper = userHelper;
            _converterHelper = converterHelper;
            _shoppingBasketRepository = shoppingBasketRepository;
            _ticketRepository = ticketRepository;
            _basketHelper = basketHelper;
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
                    var shoppingBasket = await _shoppingBasketRepository.GetShoppingBasketAsync(user.Email);

                    if (shoppingBasket != null)
                    {

                        shoppingBasketWithUser.ShoppingBasket = shoppingBasket;
                    }

                }
                else
                {
                    shoppingBasketWithUser.ShoppingBasket = new ShoppingBasket();

                }
            }
            else
            {
                shoppingBasketWithUser.ShoppingBasket = _basketHelper.GetBasket(HttpContext.Session);
                shoppingBasketWithUser.NewUser = new RegisterNewUserViewModel();
            }

            return View(shoppingBasketWithUser);
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

            var ticket = await _ticketRepository.GetTicketBySeatIdAsync(shoppingBasketTicket.SeatId, shoppingBasketTicket.Name, shoppingBasketTicket.Surname);

            return View(shoppingBasketTicket);
        }

        // GET: ShoppingBaskets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ShoppingBaskets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShoppingBasketTickets(TicketViewModel model)
        {
            if (ModelState.IsValid)
            {
                var basketTicket = _converterHelper.ToShoppingBasketTicket(model, true);

                User? user = null;

                if (User.Identity.IsAuthenticated)
                {
                    user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
                }

                ShoppingBasket shoppingBasket = new ShoppingBasket();

                if (user != null) 
                {
                    shoppingBasket = await _shoppingBasketRepository.AddTicketToShoppingBasket(basketTicket, user);
                }
                else
                {
                    shoppingBasket = _basketHelper.GetBasket(HttpContext.Session);
                    shoppingBasket.Tickets.Add(basketTicket);
                    _basketHelper.SaveBasket(HttpContext.Session, shoppingBasket);
                }

                var shoppingBasketWithUserViewModel = new ShoppingBasketWithUserViewModel
                {
                    ShoppingBasket = shoppingBasket,
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
            ShoppingBasket basket = new ShoppingBasket();
            User user = null;

            if (User.Identity.IsAuthenticated)
            {
                user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
                basket = await _shoppingBasketRepository.GetShoppingBasketAsync(user.Email);
            }
            else
            {
                basket = _basketHelper.GetBasket(HttpContext.Session);
            }

            var shoppingBasketWithUserViewModel = new ShoppingBasketWithUserViewModel
            {
                ShoppingBasket = basket,
                NewUser = user == null ? new RegisterNewUserViewModel() : null
            };

            if (basket == null)
            {
                return View("NoShoppingBasket");
            }

            return View(shoppingBasketWithUserViewModel);

        }     
         
    }
}
