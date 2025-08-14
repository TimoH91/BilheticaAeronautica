using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Helpers;
using BilheticaAeronauticaWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace BilheticaAeronauticaWeb.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]

    public class FlightsController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly ITokenService _tokenService;
        private readonly IMailHelper _mailHelper;
        private readonly IFlightRepository _flightRepository;

        public FlightsController(IUserHelper userHelper, ITokenService tokenService, IMailHelper mailHelper, IFlightRepository flightRepository)
        {
            _userHelper = userHelper;
            _tokenService = tokenService;
            _mailHelper = mailHelper;
            _flightRepository = flightRepository;
        }

        [HttpGet("GetAllFutureFlights")]
        public IActionResult GetAllFutureFlights()
        {
            var flights = _flightRepository.GetAllFutureFlights();

            return Ok(flights);
        }

    }
}
