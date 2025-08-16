using BilheticaAeronauticaWeb.Entities;
using BilheticaAeronauticaWeb.Helpers;
using BilheticaAeronauticaWeb.Models;
using BilheticaAeronauticaWeb.Services;
using BilheticaAeronauticaWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace BilheticaAeronauticaWeb.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IUserHelper _userHelper;
        private readonly IOrderRepository _orderRepository;
        


        public OrdersController(IOrderService orderService, IUserHelper userHelper, IOrderRepository orderRepository)
        {
            _orderService = orderService;
            _userHelper = userHelper;
            _orderRepository = orderRepository;
        }

        [HttpPost("ConfirmOrder")]
        public async Task<IActionResult> ConfirmOrder([FromBody] List<ShoppingBasketTicket> shoppingBasketTickets)
        {
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (shoppingBasketTickets.Any())
            {

                var userId = shoppingBasketTickets.First().UserId;
                var user = await _userHelper.GetUserByIdAsync(userId);

                var order = _orderService.ConvertToOrder(shoppingBasketTickets, user);

                await _orderRepository.CreateAsync(order);

                var tickets = await _orderService.ConvertAdultAndChildTickets(shoppingBasketTickets, user);

                tickets = await _orderService.AddTicketsAsync(tickets, order);

                var infantTickets = await _orderService.ConvertInfantTickets(tickets, shoppingBasketTickets, user);

                infantTickets = await _orderService.AddTicketsAsync(infantTickets, order);

                return Ok(new { message = "Tickets Received" });

            }

            return BadRequest(new { message = "Tickets not send" });
        }


    }
}
