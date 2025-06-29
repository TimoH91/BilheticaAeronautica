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

namespace BilheticaAeronauticaWeb.Controllers
{
    public class OrdersController : Controller
    {
        private readonly DataContext _context;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderService _orderService;
        private readonly IConverterHelper _converterHelper;
        private readonly IShoppingBasketRepository _shoppingBasketRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IUserHelper _userHelper;
        private readonly IBasketHelper _basketHelper;

        public OrdersController(DataContext context, IOrderRepository orderRepository, IConverterHelper converterHelper,
            IShoppingBasketRepository shoppingBasketRepository, ITicketRepository ticketRepository, IUserHelper userHelper,
            IOrderService orderService, IBasketHelper basketHelper)
        {
            _context = context;
            _orderRepository = orderRepository;
            _converterHelper = converterHelper;
            _shoppingBasketRepository = shoppingBasketRepository;
            _ticketRepository = ticketRepository;
            _userHelper = userHelper;
            _orderService = orderService;
            _basketHelper = basketHelper;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var orders = await _orderRepository.GetOrdersByUserAsync(User.Identity.Name);

            if (orders == null)
            {
                return new NotFoundViewResult("");
            }

            return View(orders);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("TicketNotFound");
            }

            return null;

            //Potentially this method will servce for Details here

            //ShoppingBasketTicket shoppingBasketTicket = await _shoppingBasketRepository.GetShoppingBasketTicketAsync(id.Value);

            //if (shoppingBasketTicket == null)
            //{
            //    return new NotFoundViewResult("TicketNotFound");
            //}

            //var ticket = await _ticketRepository.GetTicketBySeatIdAsync(shoppingBasketTicket.SeatId, shoppingBasketTicket.Name, shoppingBasketTicket.Surname);

            //return View("~/Views/Tickets/Details.cshtml", ticket);
        }

        // GET: Orders/Create
        public async Task<IActionResult> Create()
        {
           var shoppingBasket1 = await _shoppingBasketRepository.GetShoppingBasketAsync(User.Identity.Name);

            return View(shoppingBasket1);
        }

        public async Task<IActionResult> ShoppingBasket(TicketViewModel model)
        {
            //if (ModelState.IsValid)
            //{
            //    var basketTicket = _converterHelper.ToShoppingBasketTicket(model, true);

            //    var shoppingBasket = await _orderRepository.AddTicketToShoppingBasket(basketTicket, User.Identity.Name);

            //    //shoppingBasket.Tickets.Add(basketTicket);

            //    return View(shoppingBasket);
            //}

            return RedirectToAction("Create", "Tickets");
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Purchase(ShoppingBasketWithUserViewModel model)
        {
            Order order = new Order();
            Decimal totalPrice = 0;
            List<Ticket> tickets = new List<Ticket>();

            var user = new User();

            if (model.NewUser != null)
            {
                user = new User
                {
                    FirstName = model.NewUser.FirstName,
                    LastName = model.NewUser.LastName,
                    Email = model.NewUser.Username,
                    UserName = model.NewUser.Username,
                };

                model.ShoppingBasket = _basketHelper.GetBasket(HttpContext.Session);

                await _userHelper.AddUserAsync(user, model.NewUser.Password);
                await _userHelper.AddUserToRoleAsync(user, "Customer");

                var loginViewModel = new LoginViewModel
                {
                    Password = model.NewUser.Password,
                    RememberMe = false,
                    Username = model.NewUser.Username,
                };

                var result2 = await _userHelper.LoginAsync(loginViewModel);
            }
            else
            {
                user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
                model.ShoppingBasket = await _shoppingBasketRepository.GetShoppingBasketAsync(user.Email);
            }

            foreach (ShoppingBasketTicket basketTicket in model.ShoppingBasket.Tickets)
            {
                //TODO: will this work for different ticket types?

                Ticket ticket = await _converterHelper.BasketToTicket(basketTicket);

                ticket.UserId = user.Id;

                totalPrice += ticket.Price;

                tickets.Add(ticket);
            }

            order.TotalPrice = totalPrice;
            order.User = user;
            order.OrderDate = DateTime.Now;

            await _orderRepository.CreateAsync(order);

            foreach (Ticket ticket in tickets)
            {
                ticket.OrderId = order.Id;
            }


            await _ticketRepository.CreateRangeAsync(tickets);

            if (model.NewUser == null)
            {
                await _orderService.ClearShoppingBasket(model.ShoppingBasket);
            }
            else
            {
                _basketHelper.ClearBasket(HttpContext.Session);
            }

                return View("ThankyouForBooking");
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderDate,TotalPrice,Payment")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
