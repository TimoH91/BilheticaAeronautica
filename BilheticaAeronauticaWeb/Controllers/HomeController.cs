using System.Diagnostics;
using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Entities;
using BilheticaAeronauticaWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BilheticaAeronauticaWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAirportRepository _airportRepository;
        private readonly IFlightRepository _flightRepository;

        public HomeController(ILogger<HomeController> logger,
            IAirportRepository airportRepository
            ,IFlightRepository flightRepository  )
        {
            _airportRepository = airportRepository;
            _logger = logger;
            _flightRepository = flightRepository;
        }

        public IActionResult Index()
        {
            ViewBag.Airports = _airportRepository.GetComboAirports();
            return View();
        }

        public async Task<IActionResult> SearchFlightsWithAirports(int originAirportId, int destinationAirportId)
        {

            var flights = new RoundTripFlightViewModel
            {
                OutboundFlights = await _flightRepository.GetFlightsByOriginAndDestination(originAirportId, destinationAirportId)
            };

            return View("Views/Flights/FlightSearch.cshtml", flights);

        }

        public async Task<IActionResult> SearchFlightsWithAirportsAndDate(int originAirportId, int destinationAirportId, DateTime departureDate)
        {

            var flights = new RoundTripFlightViewModel
            {
                OutboundFlights = await _flightRepository.GetFlightsByOriginDestinationAndDate(originAirportId, destinationAirportId, departureDate)
            };

            return View("Views/Flights/FlightSearch.cshtml", flights);

        }


        public async Task<IActionResult> SearchFlightsWithAirportsAndReturn(int originAirportId, int destinationAirportId, DateTime departureDate, DateTime returnDate)
        {
            var roundTrip = new RoundTripFlightViewModel
            {
                OutboundFlights = await _flightRepository.GetFlightsByOriginDestinationAndDate(originAirportId, destinationAirportId, departureDate),
                ReturnFlights = await _flightRepository.GetFlightsByOriginDestinationAndDate(destinationAirportId, originAirportId, returnDate)
            };

            return View("Views/Flights/FlightSearch.cshtml", roundTrip);

        }

        public IActionResult Flights()
        {
            return View();
        }

        public IActionResult Airports()
        {
            return View();
        }

        public IActionResult Tickets()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Terms()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("error/404")] 
        public IActionResult Error404()
        {
            return View();
        }
    }
}
