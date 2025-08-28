using BilheticaAeronauticaWeb.Data;
using Microsoft.AspNetCore.Mvc;

namespace BilheticaAeronauticaWeb.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AirportsController : Controller
    {
        private readonly IAirportRepository _airportRepository;


        public AirportsController(IAirportRepository airportRepository)
        {
            _airportRepository = airportRepository;
        }

        [HttpGet("GetAllAirports")]
        public IActionResult GetAllAirports()
        {
            var airports = _airportRepository.GetAll();

            return Ok(airports);
        }
    }
}
