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

        //[HttpGet("GetSeats")]
        //public async Task<IActionResult> GetSeats([FromBody] GetSeatsRequest request)
        //{
        //    var seats = await _seatRepository.GetAvailableSeatsByFlight(request.FlightId);

        //    return Ok(seats);
        //}

        [HttpGet("GetSeats/{flightId}")]
        public async Task<IActionResult> GetSeats(int flightId)
        {
            var seats = await _seatRepository.GetAvailableSeatsByFlight(flightId);

            return Ok(seats);
        }

        //[HttpPut("holdseat")]
        //public async Task<IActionResult> HoldSeat(int seatId)
        //{
        //    var seat = await _seatRepository.GetByIdTrackedAsync(seatId);
        //    seat.IsHeld = true;
        //    seat.HoldingTime = DateTime.Now;
        //    await _seatRepository.UpdateAsync(seat);
        //    return Ok(new { message = "Seat held" });
        //}

        [HttpPost("{seatId}/hold")]
        public async Task<IActionResult> HoldSeat(int seatId)
        {
            var seat = await _seatRepository.GetByIdTrackedAsync(seatId);
            if (seat == null)
                return NotFound(new { message = "Seat not found" });

            if (seat.IsHeld)
                return BadRequest(new { message = "Seat is already held" });

            seat.IsHeld = true;
            seat.HoldingTime = DateTime.UtcNow; // server sets the holding time

            await _seatRepository.UpdateAsync(seat);

            return Ok(new { message = "Seat held successfully" });
        }


        public class GetSeatsRequest
        {
            public int FlightId { get; set; }
        }
    }
}
