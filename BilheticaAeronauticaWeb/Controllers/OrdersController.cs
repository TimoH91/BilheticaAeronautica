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
        private readonly DataContext _context;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderService _orderService;
        private readonly IConverterHelper _converterHelper;
        private readonly IShoppingBasketRepository _shoppingBasketRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IUserHelper _userHelper;
        private readonly IBasketHelper _basketHelper;
        private readonly IMailHelper _mailHelper;

        public OrdersController(DataContext context, IOrderRepository orderRepository, IConverterHelper converterHelper,
            IShoppingBasketRepository shoppingBasketRepository, ITicketRepository ticketRepository, IUserHelper userHelper,
            IOrderService orderService, IBasketHelper basketHelper, IMailHelper mailHelper)
        {
            _context = context;
            _orderRepository = orderRepository;
            _converterHelper = converterHelper;
            _shoppingBasketRepository = shoppingBasketRepository;
            _ticketRepository = ticketRepository;
            _userHelper = userHelper;
            _orderService = orderService;
            _basketHelper = basketHelper;
            _mailHelper = mailHelper;
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
                return new NotFoundViewResult("OrderNotFound");
            }

            var order = await _orderRepository.GetByIdAsync(id.Value);

            if (id == null)
            {
                return new NotFoundViewResult("OrderNotFound");
            }

            return View(order);
        }

        // GET: Orders/Create
        public async Task<IActionResult> Create()
        {
           var shoppingBasket1 = await _shoppingBasketRepository.GetShoppingBasketAsync(User.Identity.Name);

            return View(shoppingBasket1);
        }

        //public async Task<IActionResult> ShoppingBasket(TicketViewModel model)
        //{
        //    return RedirectToAction("Create", "Tickets");
        //}

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

                var result = await _userHelper.AddUserAsync(user, model.NewUser.Password);

                if (result != IdentityResult.Success)
                {
                    ModelState.AddModelError(string.Empty, "The user couldn't be created.");
                    return View(model);
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

                    Response response = _mailHelper.SendEmail(model.NewUser.Username, "Email confirmation", $"<h1>Email Confirmation</h1>" +
                                                                             $"To allow the user, " +
                                                                            $"please click in this link:</br></br><a href = \"{tokenLink}\">Confirm Email</a>");


                    if (response.IsSuccess)
                    {

                        ViewBag.Message = "The instructions to allow you user has been sent to email";
                        //return View(model);
                    }


                    ModelState.AddModelError(string.Empty, "The user couldn't be logged.");

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
                ticket.Order = order;
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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

    }
}
