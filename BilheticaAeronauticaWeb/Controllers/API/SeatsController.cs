using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BilheticaAeronauticaWeb.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeatsController : Controller
    {
        private readonly ISeatRepository _seatRepository;

        public SeatsController(ISeatRepository seatRepository)
        {
            _seatRepository = seatRepository;
        }

        [HttpPost("GetSeats")]
        public async Task<IActionResult> GetSeats([FromBody] GetSeatsRequest request)
        {
            var seats = await _seatRepository.GetAvailableSeatsByFlight(request.FlightId);

            return Ok(seats);
        }

        public class GetSeatsRequest
        {
            public int FlightId { get; set; }
        }
    }
}
