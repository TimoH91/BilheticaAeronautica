using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BilheticaAeronauticaWeb.Controllers.API
{


    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TicketsController : Controller
    {
            private readonly ITicketRepository _ticketRepository;
            private readonly IUserHelper _userHelper;
            private readonly ILogger<TicketsController> _logger;

            public TicketsController(ITicketRepository ticketRepository
                ,IUserHelper userHelper,
                ILogger<TicketsController> logger)
            {
                _ticketRepository = ticketRepository;
                _userHelper = userHelper;
                _logger = logger;
            }


        //[HttpGet]
        //public async Task<IActionResult> GetTickets()
        //{
        //    var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

        //    if (user == null)
        //    {
        //        return Unauthorized();
        //}

        //    return Ok(await _ticketRepository.GetFutureTickets(user));
        //}

        [HttpGet]
        public async Task<IActionResult> GetTickets()
        {
            var isAuthenticated = User.Identity?.IsAuthenticated ?? false;
            var userName = User.Identity?.Name ?? "null";

            _logger.LogInformation("GetTickets called. Authenticated: {IsAuthenticated}, UserName: {UserName}", isAuthenticated, userName);

            if (!isAuthenticated)
            {
                _logger.LogWarning("Unauthorized request to GetTickets");
                return Unauthorized();
            }

            var user = await _userHelper.GetUserByEmailAsync(userName);
            if (user == null)
            {
                _logger.LogWarning("User not found in database for UserName: {UserName}", userName);
                return Unauthorized();
            }

            var tickets = await _ticketRepository.GetFutureTickets(user);
            _logger.LogInformation("Returning {Count} tickets for user {UserName}", tickets.Count(), userName);

            return Ok(tickets);
        }



    }
}
