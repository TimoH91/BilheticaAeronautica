using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Entities;

namespace BilheticaAeronauticaWeb.Services
{
    public class FlightService : IFlightService
    {
        private readonly IAirplaneRepository _airplaneRepository;
        private readonly ISeatRepository _seatRepository;

        public FlightService(IAirplaneRepository airplaneRepository, ISeatRepository seatRepository)
        {
            _airplaneRepository = airplaneRepository;
            _seatRepository = seatRepository;
        }

        public async Task<List<Seat>> CreateSeatsForFlightAsync(Flight flight)
        {
            var airplane = await _airplaneRepository.GetByIdAsync(flight.AirplaneId);

            if (airplane == null)
            {
                throw new Exception("Airplane not found.");
            }

            var seats = new List<Seat>();

            for (int row = 1; row <= airplane.Rows; row++)
            {
                for (int col = 1; col <= airplane.SeatsPerRow; col++)
                {
                    seats.Add(new Seat
                    {
                        Flight = flight,
                        Row = row,
                        Column = col,
                        Occupied = false
                    });
                }
            }

            await _seatRepository.CreateRangeAsync(seats);

            return seats;
        }
    }
}
