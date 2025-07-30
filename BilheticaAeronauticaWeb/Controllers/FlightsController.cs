using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Entities;
using BilheticaAeronauticaWeb.Helpers;
using BilheticaAeronauticaWeb.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using BilheticaAeronauticaWeb.Services;
using System.Diagnostics.Metrics;

namespace BilheticaAeronauticaWeb.Controllers
{
    
    public class FlightsController : Controller
    {
        private readonly IFlightRepository _flightRepository;
        private readonly IAirplaneRepository _airplaneRepository;
        private readonly IAirportRepository _airportRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly IFlightService _flightService;
        private readonly ISeatRepository _seatRepository;
        private readonly ITicketRepository _ticketRepository;

        public FlightsController(IFlightRepository flightRepository,
            IAirplaneRepository airplaneRepository,
            IAirportRepository airportRepository,
            IConverterHelper converterHelper,
            IFlightService flightService,
            ISeatRepository seatRepository,
            ITicketRepository ticketRepository)
        {
            _flightRepository = flightRepository;
            _airplaneRepository = airplaneRepository;
            _airportRepository = airportRepository;
            _converterHelper = converterHelper;
            _flightService = flightService;
            _seatRepository = seatRepository;
            _ticketRepository = ticketRepository;
        }

        // GET: Flights
        public async Task<IActionResult> Index()
        {

            var flights = _flightRepository.GetAll().ToList();

            var flattenedFlights = flights.Select(f => new
            {
                Id = f.Id,
                Origin = f.OriginAirport.Name,
                Destination = f.DestinationAirport.Name,
                Airplane = f.Airplane.Name,
                Layover = f.Layover?.Name,
                BasePrice = f.BasePrice,
                Duration = f.Duration,
                Time = f.Time,
                Date = f.Date
                
            }).ToList();


            ViewBag.pivotData = flattenedFlights;

            return View(_flightRepository.GetAll().OrderBy(a => a.Id));
        }

        [Authorize(Roles = "Staff")]
        // GET: Flights/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("FlightNotFound");
            }

            var flight = await _flightRepository.GetByIdAsync(id.Value);


            if (flight == null)
            {
                return new NotFoundViewResult("FlightNotFound");
            }

            var seats = await _seatRepository.GetAvailableSeatsByFlight(flight.Id);
            var tickets = await _ticketRepository.GetTicketsByFlightIdAsync(flight.Id);

            ViewBag.adultTickets = tickets.Where(t => t.Type == PassengerType.Adult).Count();
            ViewBag.infantTickets = tickets.Where(t => t.Type == PassengerType.Infant).Count();
            ViewBag.childTickets = tickets.Where(t => t.Type == PassengerType.Child).Count();

            ViewBag.availableSeats = seats.Count();

            return View(flight);
        }

        [Authorize(Roles = "Staff")]
        // GET: Flights/Create
        public IActionResult Create()
        {
            ViewBag.Airplanes = _airplaneRepository.GetComboAirplanes();
            ViewBag.Airports = _airportRepository.GetComboAirports();
            
            return View(new FlightViewModel
            {
                Date = DateTime.Today
            });

        }

        // POST: Flights/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FlightViewModel model)
        {
            if (ModelState.IsValid)
            {
                var flight = _converterHelper.ToFlight(model, true);

                try
                {
                    await _flightRepository.CreateAsync(flight);


                    await _flightService.CreateSeatsForFlightAsync(flight);

                    return RedirectToAction(nameof(Index));

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
            return View(model);
        }

        [Authorize(Roles = "Staff")]
        //GET: Flights/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("FlightNotFound");
            }

            var flight = await _flightRepository.GetByIdAsync(id.Value);

            if (flight == null)
            {
                return new NotFoundViewResult("FlightNotFound");
            }

            var model = _converterHelper.ToFlightViewModel(flight);

            ViewBag.Airplanes = await GetAirplanesViewBag(flight);
            ViewBag.Airports = _airportRepository.GetComboAirports();

            return View(model);

        }

        //POST: Flights/Edit/5
        //To protect from overposting attacks, enable the specific properties you want to bind to.
        //For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FlightViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var flight = _converterHelper.ToFlight(model, false);

                    await _flightRepository.UpdateAsync(flight);

                    await _flightService.ReattributeSeats(flight);
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!await _flightRepository.ExistAsync(model.Id))
                    {
                        return new NotFoundViewResult("FlightNotFound"); ;
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [Authorize(Roles = "Staff")]
        //GET: Flights/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("FlightNotFound");
            }

            var flight = await _flightRepository.GetByIdAsync(id.Value);

            if (flight == null)
            {
                return new NotFoundViewResult("FlightNotFound");
            }

            return View(flight);
        }

        [Authorize(Roles = "Staff")]
        //POST: Flights/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var flight = await _flightRepository.GetByIdTrackedAsync(id);

            if (_flightService.AllowDeletion(flight))
            {
                try
                {
                    await _flightService.AlterSeatsAndTickets(flight);
                    await _flightRepository.DeleteAsync(flight);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {

                    if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                    {
                        var errorModel = new ErrorViewModel
                        {
                            ErrorTitle = $"Flight {flight.Id} provavelmente está a ser usado!!",
                            ErrorMessage = $"Flight {flight.Id} não pode ser apagado visto haverem encomendas que o usam.<br/><br/>" +
                                          $"Experimente primeiro apagar todas as encomendas que o estão a usar," +
                                          $"e torne novamente a apagá-lo"
                        };

                        return View("Error", errorModel);
                    }


                    return View("Error", new ErrorViewModel
                    {
                        ErrorTitle = "Erro de base de dados",
                        ErrorMessage = "Ocorreu um erro inesperado ao tentar apagar o aeroporto."
                    });

                }
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<List<SelectListItem>> GetAirplanesViewBag(Flight flight)
        {
            var airplanes = await _airplaneRepository.GetAvailableAirplanes(flight);

            var selectList = airplanes.Select(airplane => new SelectListItem
            {
                Value = airplane.Id.ToString(),
                Text = $"Model: {airplane.Name} Manufacturer: {airplane.Manufacturer}"
            }).ToList();

            return selectList;
        }


        [HttpGet]
        [Route("Flights/GetFlights")]
        public JsonResult GetFlights()
        {
            var flights = _flightRepository.GetAll()
                    .Select(f => new
                    {
                        f.Id,
                        f.Date,
                        f.Time,
                        f.Duration,
                        f.BasePrice,
                        Origin = f.OriginAirport.Name,
                        Destination = f.DestinationAirport.Name,
                        AirplaneModel = f.Airplane.Name,
                        AirplaneManufacturer = f.Airplane.Manufacturer
                    })
                    .ToList(); 

            return Json(flights);
        }





    }
}
