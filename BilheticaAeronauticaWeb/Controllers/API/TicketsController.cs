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

            public TicketsController(ITicketRepository ticketRepository
                ,IUserHelper userHelper)
            {
                _ticketRepository = ticketRepository;
                _userHelper = userHelper;
            }


            [HttpGet]
            public async Task<IActionResult> GetTickets()
            {
                var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

                if (user == null)
                {
                    return Unauthorized();
            }

                return Ok(await _ticketRepository.GetFutureTickets(user));
            }
    
        }
}
