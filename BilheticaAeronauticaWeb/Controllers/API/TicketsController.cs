using BilheticaAeronauticaWeb.Data;
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

            public TicketsController(ITicketRepository ticketRepository)
            {
                _ticketRepository = ticketRepository;
            }


            [HttpGet]
            public IActionResult GetTickets()
            {
                return Ok(_ticketRepository.GetAll());
            }
    
        }
}
